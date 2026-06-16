terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">=4.0"
    }
  }
}

provider "azurerm" {
  subscription_id                 = "3b262f36-0ffe-4237-80dd-02c8b0ca188f"
  tenant_id                       = "aedfca9c-557f-49eb-808d-c5d63ab596a0"
  resource_provider_registrations = "none"
  features {}
}