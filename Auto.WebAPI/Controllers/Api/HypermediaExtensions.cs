using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace Auto.Website.Controllers.Api;

public static class HypermediaExtensions {
    public static dynamic ToDynamic(this object value) {
        IDictionary<string, object> expando = new ExpandoObject();
        var properties = TypeDescriptor.GetProperties(value.GetType());
        foreach (PropertyDescriptor property in properties) {
            if (Ignore(property)) continue;
            expando.Add(property.Name, property.GetValue(value));
        }
        return (ExpandoObject)expando;
    }

    private static bool Ignore(PropertyDescriptor property) {
        if (property.Name == "LazyLoader") return (true);
        return property.Attributes.OfType<Newtonsoft.Json.JsonIgnoreAttribute>().Any();
    }
}

/*R435MHW
    R444CRJ
R461GEP
    R468DXU
R487MYD
    R491VBC
R497DLN
    R511FRU*/