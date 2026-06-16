rg_name     = "rg-trainfinder-qa"
location    = "norwayeast"
environment = "qa"

tags = {
  environment = "qa"
  project     = "TrainFinder"
}

# SQL Server + Database (Basic tier ~$5/month)
sql = {
  server_name = "sql-trainfinder-qa-001"
  db_name     = "sqldb-trainfinder-qa-001"
  sku_name    = "Basic"
  admin_login = "sqladmin"
}

# Key Vault
key_vault_name = "kv-trainfinder-qa-001"

# Storage Account
storage_account_name = "sattrainfinderqa001"

# App Service (F1 = Free tier)
app_service = {
  plan_name = "asp-trainfinder-qa-001"
  app_name  = "app-trainfinder-qa-001"
  sku_name  = "F1"
}
