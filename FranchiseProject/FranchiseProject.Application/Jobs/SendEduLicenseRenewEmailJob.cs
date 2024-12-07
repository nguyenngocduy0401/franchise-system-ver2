using FranchiseProject.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Jobs
{
    public class SendEduLicenseRenewEmailJob : IJob
    {
        private readonly ILogger<SendEduLicenseRenewEmailJob> _logger;
        private readonly IDocumentService _documentService;
        public SendEduLicenseRenewEmailJob(IDocumentService documentService,
            ILogger<SendEduLicenseRenewEmailJob> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("{UtcNow}", DateTime.UtcNow);
            await _documentService.NotifyCustomersOfExpiringDocuments();
            //return Task.CompletedTask;
        }
    }
}
