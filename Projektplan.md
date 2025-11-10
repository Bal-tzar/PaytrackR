# Projektplan – PaytrackR

## 1. Projektöversikt
- **Titel:** PaytrackR – Hybrid Kubernetes-infrastruktur med CI/CD och IaC
- **Sammanfattning:**  
  Projektet syftar till att utveckla en komplett molnbaserad infrastruktur för en webbaserad applikation, PaytrackR, med fokus på DevOps och modern driftsättningsteknik. Lösningen kommer att byggas med Infrastructure as Code (IaC) via OpenTofu,
  containerisering med Docker och orkestrering i Kubernetes (k8s).  
  En Raspberry Pi 5 används som lokal testmiljö med k3s för att simulera edge computing och möjliggöra säkra, kostnadseffektiva tester innan produktion i Azure.  
  Projektets fokus ligger på att bygga en robust CI/CD-pipeline med automatiserad deployment, övervakning via Grafana + Prometheus, och Trivy för säkerhetsanalys.

## 2. Mål & leverabler
- **Huvudmål:**  
  Att bygga och demonstrera en fullt fungerande molnbaserad CI/CD-pipeline med containeriserad applikation, körbar både lokalt (Pi5) och i Azure via Kubernetes.

- **Delmål:**
  - v44: Grundarkitektur definierad + lokal utvecklingsmiljö klar.  
  - v45: Dockerimage byggd och testad lokalt.  
  - v46: k3s installerat på Raspberry Pi + första deployment testad.  
  - v47: OpenTofu-konfiguration för Azure-infrastruktur klar.  
  - v48: CI/CD-pipeline i GitHub Actions klar och testad end-to-end.  
  - v49: Monitoring (Grafana/Prometheus) och säkerhetesanalys (Trivy) implementerad.  
  - v50: Slutrapport + demo redo.

## 3. Intressenter & målgrupp
- **Primär:**  
  DevOps-ingenjörer, systemadministratörer och IT-tekniker som vill förstå hur man bygger en hybrid Kubernetes-miljö från grunden med IaC och CI/CD.
- **Sekundär:**  
  Kursledare, handledare och framtida arbetsgivare inom molninfrastruktur och DevOps.

## 4. Scope
- **Ingår:**  
  - Utveckling av basapplikation i .NET (standard Blazor-app)  
  - Containerisering med Docker  
  - Lokal testmiljö via k3s på Raspberry Pi 5  
  - Fullt Kubernetes-kluster i Azure (AKS)  
  - Infrastructure as Code med OpenTofu  
  - CI/CD-pipeline via GitHub Actions  
  - Monitoring med Grafana OSS + Prometheus  
  - Säkerhetsanalys med Trivy

- **Ingår ej:**  
  - Komplett frontendutveckling eller användarautentisering  
  - Kostnadsoptimering för företagsdrift  

## 5. Milstolpar & tidsplan
| Vecka | Milstolpe | Leverabel / aktivitet |
| --- | --- | --- |
| 44 | Projektstart | Infrastrukturplan + arkitekturdiagram |
| 45 | Containerisering | Dockerimage byggd, testad och dokumenterad |
| 46 | Edge deployment | Raspberry Pi5 kör k3s med Blazor-app |
| 47 | Cloud IaC | OpenTofu-skript för Azure-infrastruktur |
| 48 | CI/CD | GitHub Actions-pipeline konfigurerad och testad |
| 49 | Monitoring | Grafana + Prometheus integrerat |
| 50 | Avslut | Slutrapport, demo och dokumentation färdigställd |

## 6. Risklogg
| Risk | Sannolikhet | Konsekvens | Åtgärd |
| --- | --- | --- | --- |
| Kubernetes-kompatibilitet på Raspberry Pi 5 (ARM64) | H | H | Bygg multi-arch image så att rätt version dras automagiskt av k8s |
| CI/CD-problem i GitHub Actions (autentisering mot Azure) | M | M | Se till att secrets hanteras korrekt |
| Tidsbrist p.g.a. för bred scope | H | H | Försök avgränsa och ta ett steg i taget |
| Resurskostnader i Azure (AKS, ACR) | M | M | Sätt budget och larm, kanske till och med script så att resurser deletas om det når en viss gräns |
| Otillräcklig dokumentation eller lärande-reflektion | M | H | För dagbok för kontinuerlig dokumentation |

## 7. Resurser & verktyg
- Raspberry Pi 5 med Ubuntu Server 24.04  
- VS Code  
- .NET 9.0 SDK  
- Docker Desktop  
- Kubernetes (k3s / AKS)  
- Azure CLI + OpenTofu  
- GitHub + GitHub Actions  
- Grafana OSS + Prometheus  
- PostgreSQL  
- Trivy

## 8. Kvalitetssäkring
- Kodgranskning via pull requests på GitHub  
- Automatiserade build- och deploytester i CI/CD-pipeline  
- Manuella tester av deployment i Pi5-miljö och Azure AKS  
- Löpande loggning av lärdomar och problem i dagbok    

## 9. Demo & presentationsplan
- **Scenario:**  
  En ny version av PaytrackR pushas till GitHub → CI/CD-pipeline bygger image, kör säkerhetsskanning, och deployar automatiskt till Kubernetes i Azure.  
  Monitoring visas i Grafana med data från Prometheus.  
- **Demo visar:**  
  - Full pipeline från commit till deployment  
  - Pipeline-loggar  
  - Dashboard för driftstatus och resursövervakning  
- **Förberedelser:**  
  Förkonfigurerade miljövariabler och en liten “version bump” i koden för att demonstrera hela flödet live.

## 10. Godkännande
- **Student:** Otto de Maré, 2025-11-04  
- **Handledare:** 
- **Kommentarer:** 
