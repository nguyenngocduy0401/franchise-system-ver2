using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IQuizDetailRepository
    {
        void HardRemoveRange(List<QuizDetail> entities);
        Task<List<QuizDetail>> GetByQuizId(Guid quizId);
        Task AddRangeAsync(List<QuizDetail> entities);
    }
}
