# Rapport Obelix
 
## 1. Inleiding
Dit rapport beschrijft het de implementatie en de resultaten van reinforcement learning a.d.h.v een simulatie gebouwd in Unity met het ML-Agents. Het doel is om een virtuele agent (Obelix) te trainen om zelfstandig menhirs op te pakken en af te leveren op aangewezen bestemmingen.
Het rapport is bedoeld voor medestudenten en lectoren binnen de opleiding die vertrouwd zijn met de basisbegrippen van machine learning en Unity.
De simulatie doorloopt de volgende cyclus: Obelix observeert zijn omgeving via ray perception sensoren, kiest een actie (bewegen of draaien), ontvangt een beloning op basis van zijn prestatie, en past zijn strategie aan. Dit proces wordt herhaald over honderdduizenden stappen totdat de agent het gewenste gedrag vertoont.
## 2. Methoden
Hier bespreken we de Behavior Parameters die het gedrag van de agent definiëren, en het Agent-script dat de logica van observaties, acties en beloningen implementeert.
### 2.1 Behavior Parameters
De Behavior Parameters component configureert hoe de agent communiceert met het neurale netwerk. De volgende instellingen zijn ingevoerd:
Behavior Name	Obelix
Vector Observation Space Size	3 
Stacked Vectors	1
Continuous Actions	2 (bewegen + draaien)
Discrete Branches	0
Behavior Type	Default 
Decision Period	5 (elke 5 frames een beslissing)

Daarnaast is een Ray Perception Sensor 3D component toegevoegd die de visuele perceptie van de agent is. Deze sensor zendt raycasts uit die objecten met de tags “Menhir” en “Destination” detecteren. De straallengte is zo ingesteld dat Obelix objecten over het bijna volledige speelveld kan waarnemen.
### 2.2 Agent Script — Override Methods
Het ObelixAgent-script erft van de Agent-klasse uit het ML-Agents framework en implementeert de volgende override methods:
- Initialize()
Wordt eenmalig aangeroepen bij het laden van de scène.
- OnEpisodeBegin()
Wordt aangeroepen bij het begin van elke trainingsepisode. Deze methode reset de volledige staat: de snelheid en positie van Obelix worden gereset, alle eerder gespawnde objecten worden vernietigd, en nieuwe objecten worden ge-initiate. Zes destinations worden in een cirkelvormige opstelling geplaatstmet een instelbare radius, terwijl zes menhirs op willekeurige posities boven het speelveld worden gespawned zodat ze naar beneden vallen door zwaartekracht.
- CollectObservations
Verzamelt drie observaties voor het neurale netwerk: een boolean die aangeeft of Obelix een menhir draagt, en de genormaliseerde positite v/d agent. De overige observaties worden automatisch aangeleverd door de Ray Perception Sensor 3D component, die raycasts uitstuurt om menhirs en destinations in het veld te detecteren.
- OnActionReceived(ActionBuffers actions)
Ontvangt twee continue acties van het netwerk: een waarde voor vooruit/achteruit bewegen en een waarde voor links/rechts draaien, beide in het bereik [-1, 1]. De Rigidbody wordt verplaatst en geroteerd op basis van deze waarden. Elke tijdstap wordt een kleine negatieve beloning (-0.001) toegekend om efficiëntie te stimuleren. Wanneer Obelix van het speelveld valt (y < -1), ontvangt hij een straf van -1.0 en eindigt de episode.
- OnCollisionEnter(Collision collision)
Handelt botsingen af met menhirs en destinations. Bij het raken van een menhir zonder er al een te dragen, wordt de menhir opgepakt (+0.5 beloning). Bij het raken van een vrije destination met een menhir, wordt deze afgeleverd (+1.0 beloning) en verandert de destination van kleur om aan te geven dat deze bezet is. Wanneer alle zes destinations bezet zijn, ontvangt de agent een bonus van +2.0 en eindigt de episode succesvol. Ongewenste acties zoals het benaderen van een bezette destination of het proberen oppakken van een tweede menhir worden bestraft.
- Heuristic(in ActionBuffers actionsOut)
Maakt het mogelijk om de agent handmatig te besturen via het toetsenbord (pijltjestoetsen of WASD) voor te testen. De verticale as wordt gekoppeld aan de beweegactie en de horizontale as aan de draaiactie.
2.3 Beloningsstructuur
Belonings systeem:
Situatie	                                   Beloning	  Type
Menhir oppakken (zonder er al een te dragen)	+0.5	Positief
Menhir afleveren op vrije destination	         +1.0	Positief
Alle 6 menhirs afgeleverd (bonus)	            +2.0	Positief
Naar bezette destination met menhir	          -0.3	Negatief
Naar destination zonder menhir	              -0.1	Negatief
Tweede menhir proberen oppakken	              -0.2	Negatief
Elke tijdstap (efficiëntie)	                  -0.001	Negatief
Van het speelveld vallen	                     -1.0	Negatief


## 3. Resultaten
<img width="1148" height="551" alt="image" src="https://github.com/user-attachments/assets/9d5040c2-4598-4c68-a317-cddde5c4212d" /> 
<img width="1163" height="440" alt="image" src="https://github.com/user-attachments/assets/ffc1eae4-c7a9-4e85-9b53-4ffa210f04f7" />
<img width="577" height="392" alt="image" src="https://github.com/user-attachments/assets/8dd06ba7-71e3-49ef-9ed6-6ed3716d11cb" />

## 4. Conclusie

## 5. Referenties

