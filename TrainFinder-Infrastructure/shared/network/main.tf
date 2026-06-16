# Single flat nonprod VNet for dev environment

module "vnet-nonprod" {
  source        = "../modules/network"
  vnet_name     = var.net_nonprod.vnet_name
  address_space = var.net_nonprod.address_space
  subnets       = var.net_nonprod.subnets
  environment   = var.environment
  location      = var.location
  rg_name       = var.rg_name
  tags          = var.tags
}

module "private-dns-zone" {
  source                = "../modules/private_dns_zone"
  count                 = length(var.private_dns_zones.domain_names)
  rg_name               = var.rg_name
  domain_name           = var.private_dns_zones.domain_names[count.index]
  virtual_network_ids   = [module.vnet-nonprod.vnet_id]
  virtual_network_links = [module.vnet-nonprod.vnet_name]
  registration_enabled  = var.private_dns_zones.registration_enabled
}
