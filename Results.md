# 🧱 Résumé — Modèles de concurrence & architectures (C# / Channels)

## 1. Modèles de concurrence

### 1.1. Lock global
- Un seul `lock` protège tout le stock.  
- Simple à implémenter.  
- Très sûr (aucune race condition).  
- **Très mauvaise scalabilité** : toute opération bloque toutes les autres.  
- Devient vite un goulot d’étranglement sous charge.

---

### 1.2. SemaphoreSlim global
- Alternative asynchrone au lock global.  
- Compatible `async/await`.  
- Toujours un goulot d’étranglement global.  
- Légèrement plus flexible que `lock`, mais **pas plus scalable**.  
- Utile dans les API asynchrones, mais pas pour la performance.

---

### 1.3. ConcurrentDictionary
- Dictionnaire thread‑safe.  
- Permet plusieurs accès concurrents.  
- Plus lent qu’un `Dictionary` (coût de synchronisation interne).  
- Utile si **plusieurs threads** modifient le stock en même temps.  
- Inutile si un seul worker traite les commandes (Channel mono‑worker).

---

### 1.4. Lock par produit (5A)
- Un `lock` différent pour chaque produit.  
- Évite la contention globale.  
- Simple et efficace.  
- Moins performant si un produit est très sollicité.

---

### 1.5. Interlocked (5B)
- Opérations atomiques (`Interlocked.Add`, etc.).  
- Ultra rapide, lock‑free.  
- Parfait pour des entiers.  
- Pas adapté aux structures complexes.

---

### 1.6. Channel + Worker séquentiel (5C)
- Plusieurs producers écrivent dans un Channel.  
- **Un seul worker** lit et traite les commandes **dans l’ordre**.  
- Plus besoin de locks.  
- Plus besoin de `ConcurrentDictionary`.  
- Modèle monothread logique → simple, robuste, performant.

---

### 1.7. Multi‑consumers sur un Channel
- Plusieurs workers lisent le même Channel.  
- **Ne scale pas** si le stock est partagé.  
- Provoque de la contention → plus lent.  
- Utile uniquement si chaque worker a son propre état.

---

### 1.8. Sharding (4 Channels, 4 Workers)
- Le modèle scalable par excellence.  
- Chaque shard = Channel + Worker + Stock.  
- Le router envoie chaque produit vers un shard via un hash.  
- Parallélisme réel, 0 contention entre shards.  
- API publique inchangée.

---

### 1.9. 1 Channel par produit
- Séquentialité parfaite, 0 contention.  
- Parfait si peu de produits.  
- Impossible si beaucoup de produits (trop de Channels/workers).

---

## 2. Structures de données

### 2.1. Dictionary
- Suffit si **un seul worker** modifie le stock.  
- Plus rapide que `ConcurrentDictionary`.  
- Pas thread‑safe → erreurs si plusieurs workers.

---

### 2.2. ConcurrentDictionary
- Obligatoire si **plusieurs workers** modifient le même stock.  
- Plus lent que `Dictionary`.  
- Utile pour les architectures multi‑consumers.

---

### 2.3. Avec Channel + 1 Worker
- Le stock n’a **pas besoin** d’être thread‑safe.  
- Tu peux utiliser n’importe quelle structure :  
  - `Dictionary`  
  - `List`  
  - `HashSet`  
  - objets métier complexes  
  - agrégats DDD  
  - arbres, graphes, etc.

---

## 3. Pourquoi plusieurs consumers ralentissent ?

- Plusieurs workers modifient le même stock → **contention**.  
- Le scheduler doit gérer plus de threads → overhead.  
- Le cache CPU se bat pour les mêmes données → ralentissement.

Résultat :  
- latence ↑  
- throughput ↓  
- max latency ↑↑↑  

👉 **Plus de threads ≠ plus de performance.**

---

## 4. Analogie des magasins

### 1 Channel + 1 Worker
→ 1 magasin, 1 caissier, 1 stock  
→ simple, efficace, séquentiel

### Multi‑consumers sur un Channel
→ 4 caissiers qui veulent toucher à la même caisse  
→ bagarre, lenteur, erreurs

### Sharding
→ 4 magasins indépendants  
→ chacun son stock, son caissier, sa file  
→ parallélisme réel

### 1 Channel par produit
→ 1 magasin par produit  
→ parfait si peu de produits  
→ impossible si beaucoup

---

## 5. Quand utiliser quoi ?

| Situation | Modèle recommandé |
|----------|-------------------|
| Beaucoup d’opérations sur un seul produit | Channel + 1 Worker |
| Beaucoup de produits, trafic réparti | Sharding |
| Logique simple sur des entiers | Interlocked |
| Peu de produits, très hot | 1 Channel par produit |
| Besoin de simplicité et robustesse | Channel + 1 Worker |
| Besoin de parallélisme réel | Sharding |
| API async obligatoire | SemaphoreSlim global (si simple) |
| Besoin de thread‑safety multi‑workers | ConcurrentDictionary |

---

## 6. Tests importants

- **Test de cohérence** : Add – Remove = résultat final  
- **Test de séquentialité** (Channel mono‑worker)  
- **Test de contention** (multi‑workers)  
- **Test de répartition** (sharding)  
- **Benchmark** :  
  - throughput  
  - latence moyenne
  - max latency  

