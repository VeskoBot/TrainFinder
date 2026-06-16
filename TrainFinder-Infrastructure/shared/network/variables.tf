variable "rg_name" {
  description = "The Name which should be used for this Resource Group"
  type        = string
}

variable "environment" {
  description = "The environment in which the resources are created"
  type        = string
}

variable "location" {
  description = "The Azure Region where the Resource Group should exist"
  type        = string
}

variable "tags" {
  description = "A mapping of tags which should be assigned to the Resource Group"
}

variable "net_nonprod" {
  description = "Nonprod network configuration (flat VNet)"
  type = object({
    vnet_name     = string
    address_space = list(string)
    subnets = map(object({
      name              = string
      address_prefixes  = list(string)
      service_endpoints = list(string)
      delegation        = bool
      list_security_group = list(object({
        name                         = string
        priority                     = number
        direction                    = string
        access                       = string
        protocol                     = string
        source_port_ranges           = list(string)
        destination_port_ranges      = list(string)
        source_address_prefixes      = list(string)
        destination_address_prefixes = list(string)
      }))
    }))
  })
}

variable "private_dns_zones" {
  description = "Private DNS zone configuration"
  type = object({
    domain_names         = list(string)
    registration_enabled = bool
  })
}
