[Environment]::SetEnvironmentVariable("Sentry__DSN", "{SET YOUR SENTRY KEY}", "Machine")
[Environment]::SetEnvironmentVariable("ConnectionStrings__GestaoAvaliacaoWorkerContext", "Data Source={SET YOUR INSTANCE};Initial Catalog=GestaoAvaliacao;User Id=sa;Password=Ss123456; MultipleActiveResultSets=True;", "Machine")
[Environment]::SetEnvironmentVariable("GestaoAvaliacaoWorkerMongoDBSettings__ConnectionString", "mongodb://localhost:27017/GestaoAvaliacao_OMR", "Machine")
[Environment]::SetEnvironmentVariable("GestaoAvaliacaoWorkerMongoDBSettings__Database", "GestaoAvaliacao_OMR", "Machine")
[Environment]::SetEnvironmentVariable("StudentTestSentWorker_CronParameter", "0 22 * * *", "Machine")

