# Origines

Le but de ce projet est à l'origine de comprendre comment étaient calculées mes cotisations en tant que Travailleur Non Salarié (TNS) au régime de la Sécurité Sociale des Indépendants (SSI). J'ai commencé par utiliser le [simulateur officiel de l'URSSAF](https://mon-entreprise.urssaf.fr/simulateurs/ind%C3%A9pendant) mais, n'aimant pas spécialement son ergonomie et ne comprenant pas tous les calculs, j'ai décidé mi-2024 de me mettre à essayer de tout comprendre moi-même.

Au fur et à mesure que je m'enfonçais dans la complexité des calculs, je me suis dit qu'il serait intéressant d'en faire bénéficier tout le monde en mettant en place un petit site web sans prétention.

J'ai démarré avec du Blazor pour la partie front mais au vu de la plomberie inutile et de quelques expériences malencontreuses, j'ai basculé sur des quelque chose de plus "vanille".

# Aperçu du projet

Le projet est assez simple :

- Le **code métier** (règles, export Excel, etc...), ses adaptateurs (API back) et ses tests sont codés en C#.
- Le **code front** est en HTML/CSS/JS "vanille" avec un peu d'[Alpine.js](https://alpinejs.dev/) pour gérer quelques interactions et animations pour pas cher. Le but était de faire quelque chose de léger sans avoir recours à un gros framework qui n'était pas nécessaire.

Je suis partisan de limiter la [complexité accidentelle](https://fr.wikipedia.org/wiki/Complexit%C3%A9_accidentelle) tout en restant sur ma stack favorite (C# donc) ; comme ici le projet reste léger (pas de base de données, une seule page, effets et anumation limités...), j'ai pu arriver à cet objectif **en évitant les dépendances inutiles**. Il en résulte de bonnes performances, du moins je le crois, le site répondant de façon satisfaisante (moins de 3 secondes pour afficher la page sans cache sur une connexion "type 2G" de 250 kb/s et 300 ms de latence et moins d'une demi-seconde pour afficher les résultats de calcul de cotisations sur cette même connexion).

J'ai aussi fait en sorte que le site soit **lisible correctement sur diverses tailles d'écran**. Le code front-end n'étant pas mon point fort, il est possible que ce ne soit pas le cas sur tous les appareils.

Le code est écrit en **français** 😊 C'est inhabituel et c'était même une première pour moi mais, dans la mesure où le projet cible essentiellement des utilisateurs français ou francophones, que le pérmiètre métier est décrit en français et que je suis moi-même français, autant ne pas introduire d'anglais pour rien tout en tentant de traduire maladroitement des mots qui ne le sont pas originellement. Je conçois que ce choix soit perçu comme un peu bizarre ; ce n'est pas très important.

Enfin, l'export Excel est minimaliste, le but étant de faire quelque chose qui soit réutilisable du côté des utilisateurs·trices dans leurs propres feuilles Excel si besoin.

# Considérations métier

## *Hic sunt dracones*

Ce calculateur est un **simulateur**, il n'a pas vocation à remplacer le calcul fait par l'URSSAF. Cependant, il devrait donner de bonnes approximations. Surtout, les explications données en cliquant sur la petite icône de "i" bleu devrait répondre à pas mal de questions concernant le calcul de chaque type de cotisation.

La plupart des règles sont difficiles à trouver ou comprendre. Il faut soit partir des textes de loi (ce que j'ai fait par exemple pour comprendre l'assiette de calcul pour 2025 provenant de [la réforme de 2024](https://www.urssaf.fr/accueil/independant/comprendre-payer-cotisations/reforme-cotisations-independants.html)), soit passer par les explications, souvent incomplètes, de l'URSSAF, soit faire de la rétro-ingénierie sur le simulateur officiel (mais il faut partir du principe qu'il n'est pas faux, or ce n'est pas toujours le cas).
Quoi qu'il en soit, c'est assez chronophage et c'est pour cela que j'ai créé ce simulateur en essayant de synthétiser et vulgariser les explications au maximum.

## Vocabulaire

Comme chaque métier a son jargon et que j'en utilise certains mots ou concepts, je recense ici ceux qui sont non triviaux et qu'il convient d'expliquer.

- **Revenu brut** : pour les indépendant·es il s'agit en gros du résultat net (chiffre d'affaire - frais). Inclut les cotisations facultatives car, si elles sont facultatives, elles n'en sont pas moins considérées pour le calcul des cotisations.
- **Revenu net** : il s'agit du revenu brut moins les cotisations. En gros, ce que vous vous êtes viré sur votre compte bancaire personnel.
- **Assiette** : base de calcul des cotisations. En 2023 et 2024, il y a 2 assiettes distinctes : 1 pour calculer les cotisations obligatoires et 1 pour calculer la CSG et la CRDS. À partir de 2025, il n'y a plus qu'une seule assiette, constituée du revenu brut moins un abattement forfaitaire de 26%. On obtient alors des montants de CSG/CRDS nettement à la baisse, au profit des cotisations (notamment retraite) qui sont revues un peu à la hausse, compensant la baisse CSG/CRDS. Ce qui était le but de la réforme.
- **Cotisations facultatives** : les cotisations type Madelin qui sont réintégrées à l'assiette mais sont déductibles du bénéfice de l'entreprise.
- **Cotisations obligatoires** : représente les cotisations maladie, les cotisations retraite (base et complémentaire), les cotisations pour les allocations familiales et les cotisations invalidité/décès. Elles n'incluent pas les cotisations pour la formation professionnelle ni la CSG ou la CRDS.
- **PASS, PMSS** : respectivement Plafond Annuel de la Sécurité Sociale et Plafond Mensuel de la Sécurité Sociale. Il s'agit de plafonds fixés chaque année par la loi, par exemple comme ici pour 2025 : https://boss.gouv.fr/portail/accueil/actualites-boss/2024/novembre/le-plafond-de-la-securite-social.html Il s'agit de bases pour le calcul de diverses cotisations.

## Notes sur le calcul des assiettes

Comme dit ci-dessus, en 2023 et 2024, il y a 2 assiettes distinctes : 1 pour calculer les cotisations obligatoires et 1 pour calculer la CSG et la CRDS. Or, cette dernière dépend du montant des cotisations obligatoires mais la première dépend du montant de la CSG et de la CRDS. On avait donc une situation dite de "dépendance cyclique", que le simulateur résout par convergence : on calcule la première assiette un peu au hasard, on en déduit la seconde qui nous donne CSG et CRDS. Puis on recalcule la première en additionnant revenu + CSG + CRDS et on compare avec le premier calcul. Si la différence est < 1 € alors on est OK. Sinon, on recalcule la première assiette en essayant de se rapprocher de la seconde et on recommence.
C'est un peu fastidieux mais plutôt rapide et semble correspondre à ce qui est fait sur [le simulateur officiel de l'URSSAF](https://mon-entreprise.urssaf.fr/simulateurs/ind%C3%A9pendant).

Depuis 2026 le calcul est plus simple mais toujours présent.

# Contributions ?

Le code source est dispo et les issues/PR ouvertes ; je n'ai pas énormément de temps à passer sur le code, privilégiant un rythme soutenable sur le long terme plutôt que de m'emballer sur le court terme. J'ai aussi ouvert l'espace de discussions afin d'échanger sur des sujets plutôt que d'ouvrir un ticket dans les [Issues](https://github.com/GTechene/CalculateurCotisations/issues).

## Code de conduite

- **Pas de toxicité**. Certaines personnes aiment à croire qu'être un·e bon·ne dev c'est être dur ou tranchant avec les autres. Pas moi. Les meilleur·es sont humbles et respectent l'avis des autres. Cela inclut (mais n'est pas limité à) les insultes, la discrimination, les micro-agressions, la condescendance, un ton passif/agressif, l'élitisme, etc...
- **Pas de discussion politique**. Je sais que tout est politique et que ne pas vouloir discuter politique est en soi un choix politique mais j'estime que ce n'est pas l'espace pour le faire. Le prises de position anti-fascistes sont toutefois tolérées.
- J'insiste : **pas de discrimination**. Cela inclut notamment le racisme, le sexisme, l'homophobie, la transphobie, le validisme, la grossophobie, etc... même "pour rire". Je bloquerai à vue sur ces sujets.

## Soumettre du code

Quelques règles à respecter avant de soumettre du code sous forme de PR :

- Toute modification doit être **testée**. Pas de test, pas de PR.
- Toute proposition de modification doit être **justifiée**, notamment par une entrée dans les [Issues](https://github.com/GTechene/CalculateurCotisations/issues).
- Veillez à bien respecter l'esprit de l'existant : code en français, dépendances minimales, nommage parlant plutôt qu'abrégé dans la mesure du possible (ex : `cotisationsFacultatives` plutôt que `cf` ou `cotisesFac`).
- Je suis conscient que le code n'est pas parfait (mais quel code l'est et selon quels critères ?), on peut tout à fait discuter de refactoring et tout, mais il faut **rester courtois** et ne pas perdre de vue qu'il s'agit d'un projet fait sur le **temps personnel**.

# Merci !

Merci d'avoir lu jusqu'ici ! J'espère que le calculateur vous sera utile 😊
