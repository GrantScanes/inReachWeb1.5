using System.Collections.Generic;
using Infor.Model;
using Infor.Model.SchedulerServices.JobModules;

namespace inReachWebRebuild.ViewModels
{
    public class ModulesToolboxViewModel : BaseViewModel
    {
        public List<JobModuleType> Modules { get; set; }
        public string Title { get; set; }
        public string IdSuffix { get; set; }

    }
    public class ModuleToolViewModel : BaseViewModel
    {
        public JobModuleType Module { get; set; }
        public List<JobModuleType> NewComplimentaryModules { get; set; } = new List<JobModuleType>();
        public List<JobModuleType> ExistingComplimentaryModules { get; set; } = new List<JobModuleType>();
    }

    public class ModulePropertiesViewModel : BaseViewModel
    {
        public JobModuleType Module { get; set; }
    }

    public class InputViewModel : BaseViewModel
    {
        public ModuleDataProperty Property { get; set; }
        public JobModuleType Module { get; set; }
    }

    public class ModuleValidationViewModel : BaseViewModel
    {
        public JobModuleType Module { get; set; }
        public InforActionResult ValidationResult { get; set; }
    }
}