variable "rg_name" {
  description = " The Name which should be used for this Resource Group"
  type        = string
}
variable "environment" {
  description = "The environment in which the resources are created"
  type        = string
}
variable "location" {
  description = " The Azure Region where the Resource Group should exist"
  type        = string
}

variable "tags" {
  description = " A mapping of tags which should be assigned to the Resource"
}

variable "vnet_name" {
  description = " virtual network name"
  type        = string
}

variable "address_space" {
  description = " address space for virtual network "
}

variable "subnets" {
  description = "A map of subnets to create in the virtual network"
  type = map(object({
    name              = string
    address_prefixes  = list(string)
    service_endpoints = list(string)
    delegation = bool
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
}

variable "private_endpoint_policy" {
  description = "Enable private endpoint network policies"
  type        = string
  default     = "Disabled"
}
