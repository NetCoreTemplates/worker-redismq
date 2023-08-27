using System.Runtime.Serialization;
using ServiceStack;

namespace MyApp.ServiceModel;

[DataContract]
public class Hello : IReturn<HelloResponse>
{
    [DataMember]
    public string Name { get; set; }
}

[DataContract]
public class HelloResponse
{
    [DataMember]
    public string Result { get; set; }

    [DataMember]
    public ResponseStatus ResponseStatus { get; set; }
}