terraform {
    
    resource "azurerm_resource_group" "main" {
        name     = var.resource_group_name
        location = var.location
    }

    resource "azurerm_kubernetes_cluster" "main" {
        name                = var.cluster_name
        location            = azurerm_resource_group.main.location
        resource_group_name = azurerm_resource_group.main.name
        dns_prefix          = ""
        kubernetes_version  = var.kubernetes_version

    default_node_pool {
        name       = ""
        node_count = var.node_count
        vm_size    = var.node_vm_size
    }

    identity {
        type = "SystemAssigned"
  }

    resource "helm_release" "argocd" {
        name             = "argocd"
        repository       = "https://argoproj.github.io/argo-helm"
        chart            = "argo-cd"
        namespace        = "argocd"
        create_namespace = true
        version          = ""

        depends_on = [azurerm_kubernetes_cluster.main]
 }

}
