variable "domain_name" {
  description = "The name of the Private DNS Zone. Must be a valid domain name"
  type        = string
}

variable "rg_name" {
  description = "The name of the resource group in which to create the private DNS zone"
  type        = string
}

variable "tags" {
  description = "A mapping of tags to assign to the resource"
  type        = map(any)
  default     = null
}

variable "soa_record_email" {
  description = "The email contact for the SOA record"
  type        = string
  default     = null

}

variable "soa_record" {
  description = "soa record object"
  type = object({
    expire_time  = number
    minimum_ttl  = number
    refresh_time = number
    retry_time   = number
    ttl          = number
  })
  default = {
    expire_time  = 2419200
    minimum_ttl  = 10
    refresh_time = 3600
    retry_time   = 300
    ttl          = 3600
  }
}

################ Virtual Networks Links ##############

variable "virtual_network_links" {
  description = "Specifies the virtual network links"
  type        = list(string)
  default     = []
}

variable "virtual_network_ids" {
  description = "The IDs of the virtual network"
  type        = list(string)
}

variable "registration_enabled" {
  description = "Enable registration (true/false)"
  type        = bool
  default     = true
}

################ DNS A record ##############

variable "dns_a_records" {
  description = "List of DNS A records"
  type = list(object({
    name    = string
    ttl     = number
    records = list(string)
  }))
  default = []
}

################ DNS AAAA record ##############

variable "dns_aaaa_records" {
  description = "List of DNS AAAA records"
  type = list(object({
    name    = string
    ttl     = number
    records = list(string)
  }))
  default = []
}

################ DNS CNAME record ##############

variable "dns_cname_records" {
  description = "List of DNS CNAME records"
  type = list(object({
    name   = string
    ttl    = number
    record = string
  }))
  default = []

}

################ DNS MX record ##############

variable "dns_mx_records" {
  description = "List of DNS MX records"
  type = list(object({
    name = string
    ttl  = number
    records = list(object({
      preference = number
      exchange   = string
    }))
  }))
  default = []

}

################ DNS ptr record ##############

variable "dns_ptr_records" {
  description = "List of DNS PTR records"
  type = list(object({
    name    = string
    ttl     = number
    records = list(string)
  }))
  default = []

}

################ DNS srv record ##############

variable "dns_srv_records" {
  description = "List of DNS SRV records"
  type = list(object({
    name = string
    ttl  = number
    records = list(object({
      priority = number
      weight   = number
      port     = number
      target   = string
    }))
  }))
  default = []

}

################ DNS txt record ##############

variable "dns_txt_records" {
  description = "List of DNS TXT records"
  type = list(object({
    name = string
    ttl  = number
    records = list(object({
      value = string
    }))
  }))
  default = []

}
