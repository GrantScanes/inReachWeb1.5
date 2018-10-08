using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using inReachWebRebuild.Classes;
using inReachWebRebuild.Common;
using inReachWebRebuild.ViewModels;
using Infor.Model.SchedulerServices.JobModules;
using Infor.Model.SchedulerServices.ScheduleTypes;
using Newtonsoft.Json;

namespace inReachWebRebuild.Controllers
{
    public class JobsController : BaseController
    {
        private readonly ModulesToolboxViewModel _modulesToolboxViewModel = new ModulesToolboxViewModel();
        private readonly ModuleToolViewModel _moduleToolViewModel = new ModuleToolViewModel();

        private readonly ModuleValidationViewModel _moduleValidationViewModel = new ModuleValidationViewModel();


        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
           
            _modulesToolboxViewModel.ErrorDisplay = ErrorDisplay;
            _modulesToolboxViewModel.AppUserState = AppUserState;

            _moduleToolViewModel.ErrorDisplay = ErrorDisplay;
            _moduleToolViewModel.AppUserState = AppUserState;

            _moduleValidationViewModel.ErrorDisplay = ErrorDisplay;
            _moduleValidationViewModel.AppUserState = AppUserState;

            ViewData["UserState"] = AppUserState;
        }


        // GET: Jobs
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public PartialViewResult ModulesToolBoxPartialViewResult()
        {
            _modulesToolboxViewModel.Modules = ModuleManager.GetModuleTypesForWorkspace();
            return PartialView("/Views/Jobs/_ModulesToolbox.cshtml", _modulesToolboxViewModel);
        }
        [AllowAnonymous]
        public PartialViewResult ModulesInputPartialViewResult(string module, string expectedInputType, string modules)
        {
            JsonConverter[] converters =
            {
                new ScheduleTypeConverter(), new JobTypeConverter(), new ModuleTypeConverter()
            };

            var mods = JsonConvert.DeserializeObject<JobModuleTypes>(modules, new JsonSerializerSettings { Converters = converters });
            var mod = JsonConvert.DeserializeObject<JobModuleType>(module, new JsonSerializerSettings { Converters = converters });
            if (mod.ParentIds.Any())
            {
                foreach (var id in mod.ParentIds)
                {
                    mod.InputModules.Add(mods.FirstOrDefault(i=>i.Id == id));
                }
                
                mod.InitiliseForView();
            }
           

            _moduleToolViewModel.Module = mod;
            if (string.IsNullOrEmpty(expectedInputType)) return PartialView("_ModuleInput", _moduleToolViewModel);
            _moduleToolViewModel.NewComplimentaryModules = new List<JobModuleType>();
            foreach (var s in expectedInputType.Split(Convert.ToChar(",")))
            {
                foreach (var jobModuleType in ModuleManager.GetModuleTypesForWorkspace().Where(rt => rt.ExpectedReturnType == s.Trim()))
                    _moduleToolViewModel.NewComplimentaryModules.Add(jobModuleType);


            }
            _moduleToolViewModel.ExistingComplimentaryModules = new List<JobModuleType>();
            foreach (var s in expectedInputType.Split(Convert.ToChar(",")))
            {
                foreach (var jobModuleType in mods.Where(rt => rt.ExpectedReturnType == s.Trim()))
                    _moduleToolViewModel.ExistingComplimentaryModules.Add(jobModuleType);


            }
            return PartialView("_ModuleInput", _moduleToolViewModel);
        }

        [AllowAnonymous]
        public PartialViewResult ModulesInputFromModulePartialViewResult(string module)
        {
            JsonConverter[] converters =
            {
                new ScheduleTypeConverter(), new JobTypeConverter(), new ModuleTypeConverter()
            };

            var mod = JsonConvert.DeserializeObject<JobModuleType>(module, new JsonSerializerSettings { Converters = converters });
            _moduleToolViewModel.Module = mod;
             return PartialView("_ModuleInput", _moduleToolViewModel);
             
        }
        [AllowAnonymous]
        public JsonResult ModulesInputFromModule(string module)
        {
            JsonConverter[] converters =
            {
                new ScheduleTypeConverter(), new JobTypeConverter(), new ModuleTypeConverter()
            };

            var mod = JsonConvert.DeserializeObject<JobModuleType>(module, new JsonSerializerSettings { Converters = converters });
            _moduleToolViewModel.Module = mod;

            
            return this.Jsonp(mod);
        }
        [AllowAnonymous]
        public JsonResult NewModuleFromType(string type)
        {
           var mod = ModuleManager.GetModuleTypesForWorkspace().FirstOrDefault(rt => rt.Type == type);
           return this.Jsonp(mod);
        }

        [AllowAnonymous]
        public ViewResult Dynamic()
        {
            return View();
        }

        [AllowAnonymous]
        public PartialViewResult ValidateModulePartialViewResult(string module,   string modules)
        {
            JsonConverter[] converters =
            {
                new ScheduleTypeConverter(), new JobTypeConverter(), new ModuleTypeConverter()
            };

            var mods = JsonConvert.DeserializeObject<JobModuleTypes>(modules, new JsonSerializerSettings { Converters = converters });
            var mod = JsonConvert.DeserializeObject<JobModuleType>(module, new JsonSerializerSettings { Converters = converters });
            if (mod.ParentIds.Any())
            {
                foreach (var id in mod.ParentIds)
                {
                    mod.InputModules.Add(mods.FirstOrDefault(i => i.Id == id));
                }

                mod.InitiliseForView();
            }
            _moduleValidationViewModel.ValidationResult = mod.ValidateModule();
            _moduleValidationViewModel.Module = mod;
            
            return PartialView("_ModuleValidation", _moduleValidationViewModel);
        }
    }
}   