# Private DNS Zone Module

Creates Azure Private DNS zones with virtual network links and optional DNS record management.

## Usage

```hcl
module "private-dns-zone" {
  source                = "../modules/private_dns_zone"
  rg_name               = "rg-connectivity-noprod-001"
  domain_name           = "privatelink.blob.core.windows.net"
  virtual_network_ids   = [module.vnet-nonprod.vnet_id]
  virtual_network_links = [module.vnet-nonprod.vnet_name]
  registration_enabled  = false
}
```

## Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|:--------:|
| `domain_name` | Private DNS zone domain name | `string` | | yes |
| `rg_name` | Resource group name | `string` | | yes |
| `virtual_network_ids` | VNet IDs to link | `list(string)` | | yes |
| `virtual_network_links` | VNet link names | `list(string)` | `[]` | no |
| `registration_enabled` | Auto-register VMs in DNS | `bool` | `true` | no |
| `tags` | Resource tags | `map(any)` | `null` | no |
| `soa_record_email` | SOA record email | `string` | `null` | no |
| `soa_record` | SOA record settings | `object` | defaults | no |
| `dns_a_records` | A records | `list(object)` | `[]` | no |
| `dns_aaaa_records` | AAAA records | `list(object)` | `[]` | no |
| `dns_cname_records` | CNAME records | `list(object)` | `[]` | no |
| `dns_mx_records` | MX records | `list(object)` | `[]` | no |
| `dns_ptr_records` | PTR records | `list(object)` | `[]` | no |
| `dns_srv_records` | SRV records | `list(object)` | `[]` | no |
| `dns_txt_records` | TXT records | `list(object)` | `[]` | no |

## Resources Created

- `azurerm_private_dns_zone`
- `azurerm_private_dns_zone_virtual_network_link`
- `azurerm_private_dns_a_record` (optional)
- `azurerm_private_dns_aaaa_record` (optional)
- `azurerm_private_dns_cname_record` (optional)
- `azurerm_private_dns_mx_record` (optional)
- `azurerm_private_dns_ptr_record` (optional)
- `azurerm_private_dns_srv_record` (optional)
- `azurerm_private_dns_txt_record` (optional)

## Cloud Provider Reference

[Azure Private DNS](https://docs.microsoft.com/en-us/azure/dns/)
