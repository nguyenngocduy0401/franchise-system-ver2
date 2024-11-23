using FranchiseProject.Application.Interfaces;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Jobs
{
    public class SendContractRenewalEmailJob : IJob
    {
        private readonly IContractService _contractService;
        public SendContractRenewalEmailJob(IContractService contractService)
        {
            _contractService = contractService;
        }
        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
