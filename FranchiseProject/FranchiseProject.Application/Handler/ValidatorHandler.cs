using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Handler
{
    public static class ValidatorHandler
    {
        public static ApiResponse<T> HandleValidation<T>(ValidationResult validationResult)
        {
            var response = new ApiResponse<T>();

                response.Data = default(T);
                response.isSuccess = true;
                response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
            
            return response;
        }
    }
}

