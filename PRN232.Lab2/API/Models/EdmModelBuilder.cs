using System;
using API.Models;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace ODataBookStore.Models;

public static class EdmModelBuilder
{
    public static IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<Book>("Books").EntityType.HasKey(b => b.Id);
        builder.EntitySet<Press>("Presses");
        return builder.GetEdmModel();
    }
}
