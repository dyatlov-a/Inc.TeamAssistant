using Dapper;

namespace Inc.TeamAssistant.Primitives.DataAccess.Internal;

internal sealed class DataTypeBuilder : IDataTypeBuilder
{
    public IDataTypeBuilder AddJsonType<T>()
    {
        SqlMapper.AddTypeHandler(new JsonTypeHandler<T>());

        return this;
    }

    public IDataTypeBuilder AddLanguageIdType()
    {
        SqlMapper.AddTypeHandler(new LanguageIdTypeHandler());
        
        return this;
    }

    public IDataTypeBuilder AddMessageIdType()
    {
        SqlMapper.AddTypeHandler(new MessageIdTypeHandler());
        
        return this;
    }

    public IDataTypeBuilder AddDateOnlyType()
    {
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        
        return this;
    }

    public IDataTypeBuilder AddDateTimeOffsetType()
    {
        SqlMapper.AddTypeHandler(new DateTimeOffsetTypeHandler());
        
        return this;
    }
}