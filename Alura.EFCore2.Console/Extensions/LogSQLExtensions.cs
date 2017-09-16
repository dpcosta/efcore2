using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Alura.EFCore2.Curso.Extensions
{
    internal class SqlLoggerProvider : ILoggerProvider
    {
        public static ILoggerProvider Create()
        {
            return new SqlLoggerProvider();
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (categoryName == typeof(IRelationalCommandBuilderFactory).FullName)
            {
                return new SqlLogger();
            }
            return new NullLogger();
        }

        public void Dispose()
        {

        }
    }

    internal class NullLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //não faz nada
        }
    }

    internal class SqlLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            System.Console.WriteLine("");
            System.Console.WriteLine(formatter(state, exception));
            System.Console.WriteLine("");
        }
    }

    public static class LogSQLExtensions
    {

        public static void StartLogSqlToConsole(this DbContext contexto)
        {
            var serviceProvider = contexto.GetInfrastructure<IServiceProvider>();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(SqlLoggerProvider.Create());
        }
    }

}
