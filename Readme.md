# 📘 Kata Concurrence C#

## 🎯 Objectif

Ce kata a pour but d’apprendre à gérer la **concurrence en C#** à travers une série d’exercices progressifs.  
Tu vas implémenter un système de gestion de stock soumis à des accès concurrents, observer les problèmes, puis les corriger avec différentes techniques.

Ce kata couvre :

- les race conditions  
- les verrous (`lock`)  
- les collections thread-safe (`ConcurrentDictionary`)  
- la synchronisation asynchrone (`SemaphoreSlim`)  
- les tests de charge (`Parallel.For`, `Task.WhenAll`)  

---

## 🏗️ Structure du projet


├── README.md
├── src /Inventory.csproj 
│   ├── Inventory.cs
│   ├── Program.cs
│
|── tests
|
└── InventoryTests.cs


---

# 🧩 Étapes du Kata

## Étape 1 — Implémentation naïve (volontairement incorrecte)

### 🎯 Objectif
Créer une classe `Inventory` non thread-safe et lancer plusieurs threads pour provoquer des corruptions de données.

### Tâches
- Implémenter `Add`, `Remove`, `GetQuantity`
- Utiliser un `Dictionary<string, int>`
- Lancer 100 threads qui modifient le stock
- Observer les comportements anormaux :
  - quantités incohérentes  
  - valeurs négatives  
  - exceptions  
  - corruption mémoire  

---

## Étape 2 — Correction avec `lock`

### 🎯 Objectif
Sécuriser les accès au dictionnaire.

### Tâches
- Ajouter un verrou global ou granulaire
- Comparer les performances
- Vérifier que les corruptions disparaissent

---

## Étape 3 — Utilisation de `ConcurrentDictionary`

### 🎯 Objectif
Remplacer les verrous par des opérations atomiques.

### Tâches
- Utiliser `AddOrUpdate`, `TryUpdate`
- Comparer avec la version `lock`

---

## Étape 4 — Version asynchrone

### 🎯 Objectif
Simuler des opérations lentes et gérer la concurrence en mode async.

### Tâches
- Créer `InventoryAsync`
- Utiliser `SemaphoreSlim` pour limiter le parallélisme
- Tester avec `Task.WhenAll`

---

## Étape 5 — Tests de charge

### 🎯 Objectif
Valider la robustesse du code.

### Tâches
- Écrire des tests avec `Parallel.For`
- Vérifier l’état final du stock
- Détecter les race conditions restantes

---

# 🔥 Variantes avancées

- **Transactions atomiques** entre inventaires  
- **Version immutable** (copy-on-write)  
- **Actor Model** via `Channel<T>`  
- **Partitionnement** pour réduire la contention  

