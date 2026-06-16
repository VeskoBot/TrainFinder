rg_name     = "rg-connectivity-noprod-001"
location    = "norwayeast"
environment = "dev"
tags = {
  environment = "dev"
}

# Single flat nonprod VNet for dev
# Address space 10.1.0.0/22 leaves room for QA later (10.1.4.0/22)
net_nonprod = {
  vnet_name     = "vnet-nonprod"
  address_space = ["10.1.0.0/22"]
  subnets = {
    app = {
      name              = "snet-app"
      address_prefixes  = ["10.1.0.0/26"]
      service_endpoints = []
      delegation        = true
      list_security_group = [{
        name                         = "Allow_HTTPS_Inbound"
        priority                     = 100
        direction                    = "Inbound"
        access                       = "Allow"
        protocol                     = "Tcp"
        source_port_ranges           = ["*"]
        destination_port_ranges      = ["443"]
        source_address_prefixes      = ["*"]
        destination_address_prefixes = ["10.1.0.0/26"]
      }]
    }
    pe = {
      name              = "snet-pe"
      address_prefixes  = ["10.1.0.64/26"]
      service_endpoints = []
      delegation        = false
      list_security_group = [{
        name                         = "Allow_App_To_PE_Inbound"
        priority                     = 100
        direction                    = "Inbound"
        access                       = "Allow"
        protocol                     = "Tcp"
        source_port_ranges           = ["*"]
        destination_port_ranges      = ["443", "1433"]
        source_address_prefixes      = ["10.1.0.0/26"]
        destination_address_prefixes = ["10.1.0.64/26"]
      }]
    }
    mgmt = {
      name              = "snet-mgmt"
      address_prefixes  = ["10.1.0.128/27"]
      service_endpoints = []
      delegation        = false
      list_security_group = []
    }
  }
}

private_dns_zones = {
  domain_names         = ["privatelink.blob.core.windows.net", "privatelink.database.windows.net", "privatelink.vaultcore.azure.net"]
  registration_enabled = false
}
