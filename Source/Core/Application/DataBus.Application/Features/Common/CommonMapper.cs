using AutoMapper;
using DataBus.Domain;

namespace DataBus.Application;

public class CommonMapper: Profile
{
    public CommonMapper()
    {
        CreateMap<BackEndParams, ParameterModel>();

        CreateMap<BackEndRequest, ExecutionMoldel>()
        .ForMember(target=> target.Query, source => source.MapFrom(src => src.Procedimiento))
        .ForMember(target=> target.DataBaseTarget, source => source.MapFrom(src => src.LlaveBaseDatos))
        .ForMember(target=> target.CorrelationId, source => source.MapFrom(src => src.TransactionId))
        .ForMember(target=> target.ExecutionParams, source => source.MapFrom(src => src.Message));

        
    }
}
