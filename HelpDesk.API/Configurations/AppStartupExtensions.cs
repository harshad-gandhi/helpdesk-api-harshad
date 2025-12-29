using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Swashbuckle.AspNetCore.SwaggerGen;
using HelpDesk.Common.Utils;
using HelpDesk.Repositories.Implementations;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Services.Implementations;
using HelpDesk.Services.Interfaces;
using HelpDesk.Common.Constants;
using HelpDesk.API.Middleware;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interface;
using HelpDesk.Services.Hubs;

namespace HelpDesk.API.Configurations
{
    public static class AppStartupExtensions
    {

        #region  DependencyInjection 

        public static void ConfigureDependencies(this IServiceCollection services)
        {

            // Default Framework Services
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            services.AddHttpContextAccessor();

            // SignalR
            services.AddSignalR();

            // Database connection handler
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

            // Repository and service registrations (Scoped - per request)

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IInvitationService, InvitationService>();

            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectRepository, ProjectRepository>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmailOutboxRepository, EmailOutboxRepository>();
            services.AddScoped<IUserBackupCodeRepository, UserBackupCodeRepository>();

            services.AddScoped<IInvitationRepository, InvitationRepository>();

            services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();

            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAgentService, AgentService>();

            services.AddScoped<IEmailService, EmailService>();

            // services.AddHostedService<EmailBackgroundService>();

            services.AddScoped<IEmailOutboxRepository, EmailOutboxRepository>();

            services.AddScoped<IDirectMessageRepository, DirectMessageRepository>();
            services.AddScoped<IDirectMessageService, DirectMessageService>();

            services.AddScoped<IChatSessionsRepository, ChatSessionsRepository>();

            services.AddScoped<IChatsTagsMappingRepository, ChatsTagsMappingRepository>();

            services.AddScoped<IChatsTransfersRepository, ChatTransfersRepository>();

            services.AddScoped<IChatMessagesRepository, ChatMessagesRepository>();

            services.AddScoped<IChatService, ChatService>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IArticleService, ArticleService>();

            services.AddScoped<IArticleTranslationRepository, ArticleTranslationRepository>();
            services.AddScoped<IArticleTranslationService, ArticleTranslationService>();

            services.AddScoped<IKBSearchRepository, KBSearchRepository>();
            services.AddScoped<IKBSearchService, KBSearchService>();

            services.AddScoped<IArticleFeedbackRepository, ArticleFeedbackRepository>();
            services.AddScoped<IArticleFeddbackService, ArticleFeedbackService>();

            services.AddScoped<IArticleViewRepository, ArticleViewRepository>();
            services.AddScoped<IArticleViewService, ArticleViewService>();

            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ITagService, TagService>();

            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<ITicketService, TicketService>();

            services.AddScoped<ITicketTagRepository, TicketTagRepository>();
            services.AddScoped<ITicketTagService, TicketTagService>();

            services.AddScoped<ITicketWatcherRepository, TicketWatcherRepository>();
            services.AddScoped<ITicketWatcherService, TicketWatcherService>();

            services.AddScoped<ITicketEventRepository, TicketEventRepository>();
            services.AddScoped<ITicketEventService, TicketEventService>();

            services.AddScoped<IChatShortCutRepository, ChatShortCutRepository>();
            services.AddScoped<IChatShortCutService, ChatShortCutService>();

            services.AddScoped<IReportingRepository, ReportingRepository>();
            services.AddScoped<IReportingService, ReportingService>();

            services.AddScoped<IFileService, FileService>();


            services.AddScoped<IPersonsRepository, PersonsRepository>();

            services.AddScoped<IOrganizationsRepository, OrganizationsRepository>();

            services.AddScoped<IContactsService, ContactsService>();

            services.AddScoped<IChatAttachmentsRepository, ChatAttachmentsRepository>();


            services.AddScoped<IDepartmentRepository, DepartmentRepository>();


            services.AddScoped(typeof(IResponseService<>), typeof(ResponseService<>));

            // Email Background
            // services.AddHostedService<EmailBackgroundService>();

            // Token
            services.AddScoped<ITokenService, TokenService>();

            // Email Sender
            services.AddScoped<IEmailService, EmailService>();

            // Image Service
            services.AddScoped<IImageService, ImageService>();

            // AutoMapper Configuration
            services.AddAutoMapper(typeof(MappingConfig));

            // MiddleWare
            services.AddTransient<ExceptionMiddleware>();

            // Logger
            services.AddScoped<ILoggerService, LoggerService>();
        }

        #endregion

        #region CORS

        // Configure cross-origin access for frontend applications
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(SystemConstant.CORS_POLICY_NAME, policy =>
                {
                    policy.SetIsOriginAllowed(origin => true)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });
        }

        public static void UseCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors(SystemConstant.CORS_POLICY_NAME);
        }

        #endregion

        #region Swagger

        // Register Swagger for API documentation
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(SwaggerConstants.API_VERSION, new OpenApiInfo
                {
                    Title = SwaggerConstants.API_TITLE,
                    Version = SwaggerConstants.API_VERSION
                });

                options.OperationFilter<AcceptLanguageHeaderOperationFilter>();

                // Setup JWT authentication scheme in Swagger
                options.AddSecurityDefinition(SwaggerConstants.API_SECURITY_SCHEME, new OpenApiSecurityScheme
                {
                    Description = SwaggerConstants.API_SECURITY_SCHEME_DESCRIPTION,
                    Name = SwaggerConstants.API_SECURITY_SCHEME_NAME,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = SwaggerConstants.API_SECURITY_SCHEME,
                    BearerFormat = SwaggerConstants.API_SECURITY_SCHEME_FORMAT
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = SwaggerConstants.API_SECURITY_SCHEME
                            },
                            Scheme = SwaggerConstants.API_SECURITY_SCHEME,
                            Name = SwaggerConstants.API_SECURITY_SCHEME_NAME,
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
        }

        public static void UseSwaggerDocumentation(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(SwaggerConstants.API_END_POINT, SwaggerConstants.API_TITLE + SwaggerConstants.API_VERSION);
                });
            }
        }

        // Adds Accept-Language header parameter in Swagger
        public class AcceptLanguageHeaderOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                operation.Parameters ??= [];

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = SwaggerConstants.OPEN_API_PARAMETER_NAME,
                    In = ParameterLocation.Header,
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = SwaggerConstants.OPEN_API_PARAMETER_SCHEMA_TYPE,
                        Default = new Microsoft.OpenApi.Any.OpenApiString(SwaggerConstants.OPEN_API_DEFAULT_LANGUAGE)
                    },
                    Description = SwaggerConstants.OPEN_API_DESCRIPTION
                });
            }
        }

        #endregion

        #region Authentication

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {

            // Retrieve JWT key from settings
            string key = configuration.GetValue<string>(SystemConstant.JWT_KEY)!;

            // Add JWT bearer authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false; // Allow token without HTTPS (Development mode)
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key!)),
                    ValidateIssuer = true,
                    ValidIssuer = configuration[SystemConstant.JWT_ISSUER],
                    ValidateAudience = true,
                    ValidAudience = configuration[SystemConstant.JWT_AUDIENCE],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // No token lifetime buffer
                };

                // Enable token support for SignalR connections
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        Microsoft.Extensions.Primitives.StringValues accessToken = context.Request.Query["access_token"];
                        PathString path = context.HttpContext.Request.Path;

                        // Allow access token in query for SignalR hub connection
                        if (!string.IsNullOrEmpty(accessToken) &&
                           path.StartsWithSegments("/directMessageHub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        }

        #endregion

        #region Logger

        // Serilog for file and console logging
        public static void ConfigureLogger(this WebApplicationBuilder builder)
        {
            string rootPath = Directory.GetCurrentDirectory();
            string logPath = Path.Combine(rootPath, SystemConstant.LOGS_FOLDER_NAME, SystemConstant.LOGS_FILE_NAME);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
                    outputTemplate: SystemConstant.OUTPUT_TEMPLATE)
                .CreateLogger();
        }

        #endregion

        #region Localization

        public static void ConfigureLocalization(this IServiceCollection services)
        {
            services.AddLocalization();

            services.AddControllers()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                   {
                       options.DataAnnotationLocalizerProvider = (type, factory) =>
                         factory.Create(typeof(Messages));
                   });
        }

        // Configure localization options
        public static void UseLocalization(this WebApplication app)
        {
            string[] supportedCultures = [SwaggerConstants.ENGLISH, SwaggerConstants.HINDI];

            RequestLocalizationOptions localizationOptions = new()
            {
                DefaultRequestCulture = new RequestCulture(SwaggerConstants.OPEN_API_DEFAULT_LANGUAGE),
                SupportedCultures = [.. supportedCultures.Select(c => new CultureInfo(c))],
                SupportedUICultures = [.. supportedCultures.Select(c => new CultureInfo(c))],
                RequestCultureProviders =
                [
                    new AcceptLanguageHeaderRequestCultureProvider()
                ]
            };

            app.UseRequestLocalization(localizationOptions);
        }

        #endregion

        #region Middlewares

        public static void ConfigureMiddlewares(this WebApplication app)
        {
            app.UseStaticFiles();

            app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS

            app.UseRouting();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseCors(SystemConstant.CORS_POLICY_NAME);

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<DirectMessageHub>("/directMessageHub");

            app.MapHub<TicketHub>("/ticketHub");

        }

        #endregion

    }
}