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
    //[DisallowConcurrentExecution]
    public class SendContractRenewalEmailJob : IJob
    {
        private readonly ILogger<SendContractRenewalEmailJob> _logger;
        private readonly IContractService _contractService;
        public SendContractRenewalEmailJob(IContractService contractService,
            ILogger<SendContractRenewalEmailJob> logger)
        {
            _contractService = contractService;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("{UtcNow}", DateTime.UtcNow);
            await _contractService.NotifyCustomersOfExpiringContracts();
            //return Task.CompletedTask;
        }
    }
}
