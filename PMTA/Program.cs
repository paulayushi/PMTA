using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
using PMTA.Core.Consumer;
using PMTA.Core.Event;
using PMTA.Core.Handler;
using PMTA.Domain.Aggregate;
using PMTA.Domain.Command;
using PMTA.Domain.Entity;
using PMTA.Domain.Event;
using PMTA.Domain.Query;
using PMTA.Infrastructure;
using PMTA.Infrastructure.Consumer;
using PMTA.Infrastructure.DataAccess;
using PMTA.Infrastructure.Handler;
using PMTA.Infrastructure.Helper;
using PMTA.Infrastructure.Mediator;
using PMTA.Infrastructure.Mediator.Query;
using PMTA.Infrastructure.Producer;
using PMTA.Infrastructure.Repository;
using PMTA.Infrastructure.Store;
using PMTA.WebAPI.Command;
using PMTA.WebAPI.Handler;
using PMTA.WebAPI.Query;
using System.Text;
using EventHandler = PMTA.Infrastructure.Handler.EventHandler;

var builder = WebApplication.CreateBuilder(args);

BsonClassMap.RegisterClassMap<BaseEvent>();
BsonClassMap.RegisterClassMap<MemberCreatedEvent>();
BsonClassMap.RegisterClassMap<MemberUpdatedEvent>();
BsonClassMap.RegisterClassMap<TaskCreatedEvent>();

// Add services to the container.
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.Configure<MongoDBConfig>(builder.Configuration.GetSection(nameof(MongoDBConfig)));
builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));
//Add dbcontext
Action<DbContextOptionsBuilder> configureDbContext = option => option.UseSqlServer(builder.Configuration.GetConnectionString("PmtaDatabase"));
builder.Services.AddDbContext<PmtaDbContext>(configureDbContext);
builder.Services.AddSingleton<DbContextFactory>(new DbContextFactory(configureDbContext));

builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddScoped<IEventProducer, EventProducer>();
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<IEventSourcingHandler<MemberAggregate>, EventSourcingHandler>();
builder.Services.AddScoped<ICommandHandler, CommandHandler>();
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection("ConsumerConfig"));
builder.Services.AddScoped<IEventHandler, EventHandler>();
builder.Services.AddScoped<IEventConsumer, EventConsumer>();
builder.Services.AddScoped<IQueryHandler, QueryHandler>();

//Registering command handlers
var commandHandler = builder.Services.BuildServiceProvider().GetRequiredService<ICommandHandler>();
var commandDispatcher = new CommandDispatcher();
commandDispatcher.RegisterHandler<CreateMemberCommand>(commandHandler.HandleAsync);
commandDispatcher.RegisterHandler<UpdateMemberCommand>(commandHandler.HandleAsync);
commandDispatcher.RegisterHandler<CreateTaskCommand>(commandHandler.HandleAsync);
commandDispatcher.RegisterHandler<CreateUserCommand>(commandHandler.HandleAsync);
builder.Services.AddSingleton<ICommandDispatcher>(_ => commandDispatcher);

//Registering query handlers
var queryHandler = builder.Services.BuildServiceProvider().GetRequiredService<IQueryHandler>();
var memberQueryDispatcher = new MemberQueryDispatcher();
memberQueryDispatcher.RegisterHandler<GetAllMemberQuery>(queryHandler.HandleAsync);
builder.Services.AddSingleton<IQueryDispatcher<MemberEntity>>(memberQueryDispatcher);

var taskQueryDispatcher = new TaskQueryDispatcher();
taskQueryDispatcher.RegisterHandler<GetTasksByMemberIdQuery>(queryHandler.HandleAsync);
builder.Services.AddSingleton<IQueryDispatcher<TaskEntity>>(taskQueryDispatcher);

builder.Services.AddHostedService<ConsumerHostedService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                            .GetBytes(builder.Configuration.GetSection("JwtToken:SecretKey").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                }
            );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
//app.UseCors(builder => builder
//    .AllowAnyHeader()
//    .AllowAnyMethod()
//    .AllowCredentials()
//    .WithOrigins("https://localhost:4200"));
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();