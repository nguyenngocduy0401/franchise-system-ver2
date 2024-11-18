using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ContractViewModels
{
    public class ContractInputViewModel
    {
        public DateOnly? dateNow { get; set; }
        public DateOnly? monthNow { get; set; }
        public DateOnly? yearNow { get; set; }
        public string? AgencyName { get; set; }
        public string? AgencyBirthDate { get; set; }
        public string? CCCD { get; set; } //
        public string? CCCDdate { get; set; }  //
        public string? CCCDwhere { get; set; } //
        public string? Anumber { get; set; }  
        public string? KDnumber { get; set; }  
        public string? KDwhere { get; set; }   
        public string? KDdate { get; set; }    
        public string? AgencyNumber { get; set; }   
        public string? STK {  get; set; }       //
        public string? Bank {  get; set; }      //
        public string? TotalMoney {  get; set; }    //
        public string? Deposit {  get; set; }       //
        public string? DesignFee {  get; set; }     //
        public string? FranchiseFee { get; set; }   //
        public string? RevenueSharePercentage {  get; set; } //
        public int ? Duration { get; set; }//
    }
}
