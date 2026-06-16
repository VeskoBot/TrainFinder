storage_accounts = {
  "shell" = {
    name                          = "sattrainfinder001cshell"
    sku                           = "Standard"
    replication_type              = "LRS"
    public_network_access_enabled = true
  }
}

la_workspace = {
  name              = "la-trainfinder-mgmt-001"
  sku               = "PerGB2018"
  retention_in_days = 30
}
