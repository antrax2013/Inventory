❌ Le lock global n’est pas “mauvais”
Il est excellent quand :

peu de produits

opérations très courtes

faible contention

❌ Le ConcurrentDictionary n’est pas “toujours le meilleur”
Il est excellent quand :

beaucoup de produits

charge métier non triviale

contention répartie

besoin d’atomicité lock‑free

✔️ Le lock par produit est le meilleur compromis
Il combine :

simplicité

scalabilité

performance

lisibilité

absence de contention globale