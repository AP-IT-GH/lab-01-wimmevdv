# Jager vs Prooi

## 1. Inleiding
Het doel is het trainen van twee agents die samen en competitief leren: een prooi-agent (blauw) die rode blokken probeert te verzamelen, en een jager-agent (geel) die de prooi-agent probeert te vangen.
Het rapport is gericht aan medestudenten en lectoren die bekend zijn met de basisbegrippen van machine learning en Unity ML-Agents.

## 2. Methoden
### 2.1 Omgeving
Iedere episode neemt plaats op een plane die omsloten is door muren, iedere episode worden er random 5 blokken gespawned.
### 2.2 Behavior Parameters
**ProoiAgent**
- Behavior Name: Predator
- Vector Observation Space Size: 2
- Continuous Actions: 2 (move + rotate)
- Ray Perception Tags: RedBlock, Wall, Hunter
- Rays Per Direction: 10
- Max Ray Degrees: 120
- Ray Length:14
- Decision Period: 5
**HunterAgent**
- Behavior Name: Hunter
- Vector Observation Space Size2
- Continuous Actions: 2 (move + rotate)
- Ray Perception Tags: Predator, Wall, RedBlock
- Rays Per Direction:10
- Max Ray Degrees: 180
- Ray Length: 14
- Decision Period: 5
### 2.3 Agent Scripts
Beide agents hebben hun eigen script, ze erven over van MLAgent, en hebben volgende methoden.

Initialize(): initialiseert de Rigidbody en stelt MaxStep in op 5000.
OnEpisodeBegin(): reset posities en snelheden van beide agents, en spawnt nieuwe rode blokken (enkel in JagerAgent).
CollectObservations(): geeft enkel de eigen genormaliseerde x/z-positie mee. Visueel zien gaat via de Ray Perception Sensor.
OnActionReceived(): beweegt en draait de agent op basis van twee continue acties.
OnCollisionEnter(): handelt botsingen af met rode blokken, muren, en de andere agent.

De JagerAgent bevat een referentie naar de ProoiAgent, zodat bij het vangen de beloning van de prooi aangepast kan worden en beide episodes teglijk beëindigd worden. Omgekeerd bevat de ProoiAgent een referentie naar de JagerAgent om bij het verzamelen van alle blokken ook de jager-episode te beëindigen.
### 2.4 Beloningen
| Situatie  |  Agent | Beloning |
| -------- | ------- | -------- |
| Rood | blok oprapenProoi| +1.0 | 
| Alle blokken verzameld (bonus) | Prooi | +2.0
|Gevangen door Jager | Prooi | -1.0 | 
| Muur raken | Prooi | -0.1 | 
| Prooi vangen | Jager | +1.0 | 
| Prooi verliest (straf jager) | Jager | -1.0 | 
| Muur raken | Jager | -0.1 |

## 3. Resultaten
Hier bespreek ik de verschillende runs, en aanpassingen die ik in deze runs heb gedaan, om tot een "goed" einde te komen.
### 3.1 Run 1: Jager leerde niet
<img width="1198" height="511" alt="image" src="https://github.com/user-attachments/assets/c04c35cf-64ca-46ad-a620-2d11f7130f49" />
De Jager ontving enkel een beloning bij het vangen van de Prooi. Omdat de kans op toevallig vangen in het begin heel klein is, bleef de reward van de Jager dicht bij de 0 en leerde die niks. De Prooi leerde wel snel rode blokken te verzamelen, dit is volgens mij omdat het toevallig vangen van een blok sneller voorvalt, en dus resulteert in frequente beloningen.

### 3.2 Run 2: (te veel) belonen voor dicht bij prooi zijn 
<img width="1184" height="428" alt="image" src="https://github.com/user-attachments/assets/ffaec5b4-eff4-4048-9b4f-1bb57773b8c7" />
Om de Jager in de juiste richting te duwen hebben we de belong aangepast, hierdoor ontving de jager een beloning die varieerde aan de afstand tot de Prooi (* 0.05f). Dit resulteerde in een sterke stijging van de reward van de Jager, maar het gedrag dat de Jager hierna toonde was niet zoals gewenst: de Jager leerde dichtbij de Prooi te blijven zonder effectief te vangen, omdat het dichbijzijn goed beloond werd, de jager volgde dus gewoon de prooi op de voet.

Het feit dat de reward van de Jager toch daalt kan te wijten zijn aan het feit dat de prooi het beter door heeft, en dus sneller de blokken verzamelt, waardoor de episode sneller eindigt en de Jager minder rewards krijgt van er dichtbij te zijn:
<img width="595" height="379" alt="image" src="https://github.com/user-attachments/assets/ba061fae-b13f-45ba-8eea-411d181daf21" />

### 3.3 Run 3: Reward verlaagd voor Jager
<img width="1170" height="428" alt="image" src="https://github.com/user-attachments/assets/f4d59bf3-4246-4595-8150-3e66a8bb8f64" />

De reward voor het volgen werd verlaagd van 0.05f naar 0.005f. Hierdoor werd de eindbeloning van het vangen relatief belangrijker. De Jager had na een tijd hierdoor door dat hij de Prooi moest vangen. De rewards van beide agents groeien naar elkaar toe naar het einde van de training, wat volgens mij duidt op een beter competitief evenwicht, de drop in rewards die je ziet kan je dus hier aan wijden.

### 3.4 Run 4: Gelijke snelheid, geen extra reward
<img width="575" height="302" alt="image" src="https://github.com/user-attachments/assets/4ad9110c-6159-428d-82b9-ac607f922a97" />

In de laatste run werden de snelheden gelijkgesteld, en werd de reward voor het volgen volledig verwijderd, dit vertaalt zich in de grafiek naar een tijdelijke meer reward voor de Prooi, maar dit event zich na een tijd weer uit, wanneer ze zich aan elkaar aanpassen.

## 4. Conclusie
Het kan nuttig zijn om een extra reward te geven die je agent stimuleert om het gewenste eind resultaat te behalen, maar dan moet je wel opletten dat deze reward  niet te groot is dat het eindresultaat niet belangrijk meer word voor de agent. Alsook is het dus mogelijk om 2 agents tegelijk te trainen, al moet je de fysieke eigenschappen soms een beetje afstemmen tot je de sweet spot hebt gevonden waarin ze allebij leren.

## 5. Referenties
- Meneer zijn cursus


