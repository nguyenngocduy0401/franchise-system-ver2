using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.SyllabusViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface ISyllabusService
    {
        Task<ApiResponse<bool>> DeleteSyllabusByIdAsync(Guid syllabusId);
        Task<ApiResponse<SyllabusViewModel>> GetSyllabusIdAsync(Guid syllabusId);
        Task<ApiResponse<bool>> UpdateSyllabusAsync(Guid syllabusId, UpdateSyllabusModel updateSyllabusModel);
        Task<ApiResponse<bool>> CreateSyllabusAsync(CreateSyllabusModel createSyllabusModel);
    }
}
