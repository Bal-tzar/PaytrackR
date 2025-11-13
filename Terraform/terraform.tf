terraform {
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      version = "4.1.0"
    }
  }
}

provider "azurerm" {
  features {}

  subription_id = "6a5c7c80-b3f3-4b2f-aacb-8836a0f0aefd"
}
