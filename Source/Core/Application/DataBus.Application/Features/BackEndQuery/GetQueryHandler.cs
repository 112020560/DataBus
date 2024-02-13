using AutoMapper;
using DataBus.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DataBus.Application;

public class GetQueryHandler : CommonFeature, IRequestHandler<GetQueryData, IBackendResponse>
{
    private readonly ILogger<GetQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly BackEndConfiguration _backEndConfiguration;
    private readonly IDataBaseRepository _dataBaseRepository;
    public GetQueryHandler(ILogger<GetQueryHandler> logger,
                          IMapper mapper,
                          ISandBoxRepository sandBoxRepository,
                          ICacheService cacheService,
                          IOptions<BackEndConfiguration> options,
                          IDataBaseRepository dataBaseRepository) : base(sandBoxRepository, cacheService, options)
    {
        _logger = logger;
        _mapper = mapper;
        _backEndConfiguration = options.Value;
        _dataBaseRepository = dataBaseRepository;
    }
    public async Task<IBackendResponse> Handle(GetQueryData request, CancellationToken cancellationToken)
    {
        ///Asignamos el tenant
        var tenant = request.tenant ?? request.backEndRequest.Tenant ?? throw new Exception("Tenant no fue proporcionado");
        _logger.LogDebug("Begin Handler for Tenant {tenant}", request.backEndRequest.Tenant);
        ///Mapeamos el request con el modelo
        ExecutionMoldel internalModel = _mapper.Map<ExecutionMoldel>(request.backEndRequest) ?? throw new Exception("No fue posible realizar el mapeo entre el request y el model");
        ///Cargamos las propiedades del request al modelo
        var (success, connectionString) = await this.GetConnectionStringAsync(request.backEndRequest, cancellationToken);
        if (string.IsNullOrEmpty(connectionString)) throw new Exception("Connectionstring nulo o vacio");
        _logger.LogDebug("[{TransactionId}] - CONNECTIONSTRING: {conn}", request.backEndRequest.TransactionId, connectionString);

        internalModel.ConnString = connectionString;
        internalModel.ExecutionTimeOut = _backEndConfiguration.ExecutionTimeOut;

        var parameters = await this.GetSandboxParametersAsync(request.backEndRequest, cancellationToken) ?? throw new Exception("Parametros nulos o vacios");
        internalModel.Query = parameters.Procedure;
        internalModel.ConfigParams = parameters.ArrayParams;
        internalModel.EnableLog = parameters.EnableTrace == 1;
        internalModel.ParamsValidate = parameters.ValidateParams;
        //internalModel.ExistOutputParameters = internalModel.Params.Any(a => a.Direction != null && a.Direction.ToUpper().Equals("OUT"));

        _logger.LogDebug("[{TransactionId}] - STORE PROCEDURE: {Procedure}", request.backEndRequest.TransactionId, parameters.Procedure);
        _logger.LogDebug("[{TransactionId}] - PARAMETERS: {parameters}", request.backEndRequest.TransactionId, JsonConvert.SerializeObject(parameters.ArrayParams));

        var queryResponse = await _dataBaseRepository.GetExecutionAsync<object>(internalModel);

        return new BackEndResponse
        {
            ErrorMessage = "",
            Message = "Process OK",
            ResponseCode = "OK",
            ResponseData = queryResponse
        };
    }
}
