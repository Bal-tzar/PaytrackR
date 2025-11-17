terraform {
    variable "resource_group_name" {
        description = "Name of the resource group"
        type        = string
        default     = "paytrackr_rg"
    }   

    variable "location" {
        description = "Azure region for resources"
        type        = string
        default     = "eu-west-1"
    }

    variable "cluster_name" {
        description = "Name of the AKS cluster"
        type        = string
        default     = "paytrackr_cluster"
    }

    variable "kubernetes_version" {
        description = "Kubernetes version"
        type        = string
        default     = ""
    }

    variable "node_count" {
        description = "Number of nodes in default node pool"
        type        = number
        default     = 3
    }

    variable "node_vm_size" {
        description = "VM size for nodes"
        type        = string
        default     = "small-t1"
    }
}