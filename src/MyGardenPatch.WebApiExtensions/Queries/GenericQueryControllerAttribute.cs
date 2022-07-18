using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace MyGardenPatch.WebApiExtensions.Queries;

internal class GenericQueryControllerAttribute : Attribute, IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        var commandType = controller.ControllerType.GetGenericArguments()[0];

        controller.ControllerName = commandType.Name;
    }
}