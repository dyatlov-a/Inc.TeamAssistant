using Inc.TeamAssistant.Appraiser.Migrations;
using McMaster.Extensions.CommandLineUtils;

namespace Inc.TeamAssistant.Appraiser.MigrationsRunner
{
    sealed class Program
    {
        private static readonly CommandLineApplication App;

        static Program()
        {
            App = new()
            {
                Name = typeof(Program).Namespace,
                Description = "Database migration and bootstrapping tool",
            };

            App.HelpOption(inherited: true);
            App.OnExecute(App.ShowHelp);

            App.Command("create", config =>
            {
                var connectionString = config
                    .Argument<string>("ConnectionString", "ConnectionString to postgre with creating db permission")
                    .IsRequired();

                var userPassword = config.Argument<string>("UserPassword", "Password for appraiser__api user"
                ).IsRequired();

                config.OnExecute(() =>
                {
                    var manager = new DatabaseInitialManager(connectionString.ParsedValue);
                    manager.CreateDatabase(userPassword.ParsedValue);

                    Console.WriteLine("Db was created successfully");

                    return 0;
                });
            });

            App.Command("migrate", config =>
            {
                var connectionString = config
                    .Argument<string>("ConnectionString", "ConnectionString to db with managing db objects permission")
                    .IsRequired();

                config.OnExecute(() =>
                {
                    var wrapper = new MigrationRunnerWrapper(connectionString.ParsedValue);
                    wrapper.Execute(r => r.MigrateUp());

                    Console.WriteLine("Migrations was applied successfully");

                    return 0;
                });
            });
        }

        private static void Main(string[] args) => App.Execute(args);
    }
}