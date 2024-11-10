using System;
using System.Threading;
using System.Threading.Tasks;
using FranchiseProject.Application;
using FranchiseProject.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class ClassRoomBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
 

    public ClassRoomBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
      
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdateClassRoomStatusAsync();

            //24h
            //await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            //Test 30 s
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task UpdateClassRoomStatusAsync()
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

            // Lấy tất cả các ClassRoom mà có ToDate và trạng thái không phải CanFeedback
            var classRoomsToUpdate = await unitOfWork.ClassRoomRepository.GetClassRoomsWithNullStatusAndToDateAsync();

            // Chuyển sang client-side để tính toán khoảng cách ngày
           var filteredClassRooms = classRoomsToUpdate
    .Where(c => c.ToDate.HasValue 
                && (c.ToDate.Value.ToDateTime(new TimeOnly(0, 0)) - currentDate.ToDateTime(new TimeOnly(0, 0))).Days == 10)
    .ToList();

            // Cập nhật trạng thái của các ClassRoom
            foreach (var classRoom in filteredClassRooms)
            {
                classRoom.Status = ClassRoomEnumStatus.CanFeedback;
                unitOfWork.ClassRoomRepository.UpdatesAsync(classRoom);
            }

            // Lưu thay đổi
            await unitOfWork.SaveChangeAsync();
        }
    }
}
