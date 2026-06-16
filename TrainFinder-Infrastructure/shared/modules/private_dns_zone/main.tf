################## dns private zone  ##################


resource "azurerm_private_dns_zone" "example" {
  name                = var.domain_name
  resource_group_name = var.rg_name
  tags                = var.tags

  dynamic "soa_record" {
    for_each = var.soa_record_email == null ? [] : [1]
    content {
      email        = var.soa_record_email        #(Required) 
      expire_time  = var.soa_record.expire_time  #(Optional) Defaults to 2419200.
      minimum_ttl  = var.soa_record.minimum_ttl  #(Optional) Defaults to 10.
      refresh_time = var.soa_record.refresh_time #(Optional) Defaults to 3600.
      retry_time   = var.soa_record.retry_time   #(Optional) Defaults to 300.
      ttl          = var.soa_record.ttl          #(Optional) Defaults to 3600.
      tags         = var.tags
    }
  }
}



################## virtual network link ##################

resource "azurerm_private_dns_zone_virtual_network_link" "example" {
  count = length(var.virtual_network_links)

  name                  = var.virtual_network_links[count.index]
  resource_group_name   = var.rg_name
  private_dns_zone_name = azurerm_private_dns_zone.example.name
  virtual_network_id    = var.virtual_network_ids[count.index]
  registration_enabled  = var.registration_enabled
  tags                  = var.tags
}



################ DNS A record ##############

resource "azurerm_private_dns_a_record" "example" {
  count = length(var.dns_a_records)

  name                = var.dns_a_records[count.index].name
  zone_name           = azurerm_private_dns_zone.example.name
  resource_group_name = var.rg_name
  ttl                 = var.dns_a_records[count.index].ttl
  records             = var.dns_a_records[count.index].records #Alias record to IPv4 addresses
  tags                = var.tags
}

################ DNS AAAA record ##############

resource "azurerm_private_dns_aaaa_record" "test" {
  count = length(var.dns_aaaa_records)

  name                = var.dns_aaaa_records[count.index].name
  zone_name           = azurerm_private_dns_zone.example.name
  resource_group_name = var.rg_name
  ttl                 = var.dns_aaaa_records[count.index].ttl
  records             = var.dns_aaaa_records[count.index].records #Alias record to IPv6 addresses
  tags                = var.tags
}

################ DNS CNAME record ##############

resource "azurerm_private_dns_cname_record" "example" {
  count = length(var.dns_cname_records)

  name                = var.dns_cname_records[count.index].name
  zone_name           = azurerm_private_dns_zone.example.name
  resource_group_name = var.rg_name
  ttl                 = var.dns_cname_records[count.index].ttl
  record              = var.dns_cname_records[count.index].record #Link subdomain to another single record
  tags                = var.tags
}

################ DNS MX record ##############

resource "azurerm_private_dns_mx_record" "example" {
  count = length(var.dns_mx_records)

  name                = var.dns_mx_records[count.index].name
  resource_group_name = var.rg_name
  zone_name           = azurerm_private_dns_zone.example.name
  ttl                 = var.dns_mx_records[count.index].ttl

  dynamic "record" {
    for_each = var.dns_mx_records[count.index].records
    iterator = mx
    content {
      preference = mx.value["preference"]
      exchange   = mx.value["exchange"]
    }
  }
  tags = var.tags
}

################ DNS PTR record ##############

resource "azurerm_private_dns_ptr_record" "example" {
  count = length(var.dns_ptr_records)

  name                = var.dns_ptr_records[count.index].name
  zone_name           = azurerm_private_dns_zone.example.name
  resource_group_name = var.rg_name
  ttl                 = var.dns_ptr_records[count.index].ttl
  records             = var.dns_ptr_records[count.index].records #Pointer domain records
  tags                = var.tags
}

################ DNS SRV record ##############

resource "azurerm_private_dns_srv_record" "test" {
  count = length(var.dns_srv_records)

  name                = var.dns_srv_records[count.index].name
  resource_group_name = var.rg_name
  zone_name           = azurerm_private_dns_zone.example.name
  ttl                 = var.dns_srv_records[count.index].ttl
  dynamic "record" {
    for_each = var.dns_srv_records[count.index].records #SRV records
    iterator = srv
    content {
      priority = srv.value["priority"]
      weight   = srv.value["weight"]
      port     = srv.value["port"]
      target   = srv.value["target"]
    }
  }
  tags = var.tags
}

################ DNS txt record ##############

resource "azurerm_private_dns_txt_record" "test" {
  count = length(var.dns_txt_records)

  name                = var.dns_txt_records[count.index].name
  resource_group_name = var.rg_name
  zone_name           = azurerm_private_dns_zone.example.name
  ttl                 = var.dns_txt_records[count.index].ttl

  dynamic "record" {
    for_each = var.dns_txt_records[count.index].records #Text type records
    iterator = txt
    content {
      value = txt.value["value"]
    }
  }
  tags = var.tags
}
