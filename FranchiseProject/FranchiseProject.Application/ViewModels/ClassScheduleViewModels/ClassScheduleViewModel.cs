﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ClassScheduleViewModels
{
    public class ClassScheduleViewModel
    {
       public string? Id {  get; set; }
        public string? Room {  get; set; }
        public Guid? ClassId { get; set; }
        public string? ClassName {  get; set; }
       
        public string? SlotName {  get; set; }
        public string? Date {  get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? Url { get; set; }

    }
}
