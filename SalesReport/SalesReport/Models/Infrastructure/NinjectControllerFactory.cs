using SalesReport.Models.Abstract;
using SalesReport.Models.Context;
using Ninject;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace SalesReport.Infrastructure
{
    public class NinjectContollerFactory : DefaultControllerFactory
    {
        private IKernel _ninjectKernel;

        public NinjectContollerFactory()
        {
            _ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)_ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            _ninjectKernel.Bind<ISalesContainer>().To<EFSalesContainer>();
            _ninjectKernel.Bind<IEmailService>().To<EmailService>();
            _ninjectKernel.Bind<IExcelExport>().To<ExcelExport>();
        }
    }
}