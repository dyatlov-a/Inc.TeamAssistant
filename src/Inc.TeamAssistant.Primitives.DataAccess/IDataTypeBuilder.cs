using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Primitives.DataAccess;

public interface IDataTypeBuilder
{
    IDataTypeBuilder AddJsonType<T>();
    IDataTypeBuilder AddLanguageIdType();
    IDataTypeBuilder AddMessageIdType();
    IDataTypeBuilder AddDateOnlyType();
    IDataTypeBuilder AddDateTimeOffsetType();
    
    IServiceCollection Build();
}