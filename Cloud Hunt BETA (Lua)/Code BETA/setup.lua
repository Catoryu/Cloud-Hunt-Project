--Raccourcis
lg = love.graphics
keyDown = love.keyboard.isDown
color = lg.setColor
draw = lg.draw
image = lg.newImage
sound = love.audio

--Type d'affichage des pixels en agrandissement
lg.setDefaultFilter("nearest", "nearest")

--Rendre le curseur invisible sur la fenêtre
love.mouse.setVisible(false)
--
---Variables glboales---
--
--Fenêtre
wdowHgt = 500
wdowWth = wdowHgt * 1.4

--Temps
time = 0
timeFrame = 0.02
frame = timeFrame
timeCombo = 1

--Nuages
minCloudSize = 100
maxCloudSize = 300
cloudNb = 20

--Compteurs
shots = 0
fails = 0
kills = 0
comboPts = 0
score = 0

--Autres
killsLimit = cloudNb --Nombre de nuages à toucher
opacityLevel = 12.75 --Vitesse de disparition des nuages
fileText = "" --Texte contenu dans le fichier des scores
name = "" --Nom du joueur
enterName = false --Demande du nom du jouer
scoreTableRefresh = true --Rafraichissement du classement
start = false --Lancement du jeu
win = false --Victoire
pause = true --Mode pause
icon = image("assets/images/icon.png") --Image de l'application

--Mise en place de certains paramètres--
textSize = wdowHgt / 250 --Taille des textes
love.window.setMode(wdowWth, wdowHgt) --Taille de la fenêtre
love.window.setTitle("Cloud Hunt") --Nom de la fenêtre
love.window.setIcon(icon:getData()) --Icone de la fenêtre
math.randomseed(os.time()) --Pour les valeurs aléatoires
--
---Variables de l'arrière plan---
--
back = {
    x = 0 - wdowHgt / 10,
    y = 0 - wdowHgt / 5,
    img = image("assets/images/ocean.png"),
    verticalAnime = 2,
    spd = wdowHgt / 4000
}
back.imgWth = back.img:getWidth()
back.imgHgt = back.img:getHeight()
back.wth = wdowHgt * 1.2 / back.imgWth
back.hgt = wdowHgt * 1.2 / back.imgHgt
--
---Variables des nuages---
--
cloudImg = image("assets/images/cloud-running.png")
cloudImgWth = cloudImg:getWidth()
cloudImgHgt = cloudImg:getHeight()
cloudContainer = {}
for i = 0, cloudNb - 1 do
    cloud = {}
    cloud.x = wdowHgt
    cloud.y = math.random(0, wdowHgt - (cloudImgHgt * 3))
    cloud.scale = math.random(minCloudSize, maxCloudSize) / 100
    cloud.spd = 4 - cloud.scale
    cloud.direction = math.random(0, 1)
    cloud.opacity = 255
    cloud.isTouched = false
    table.insert(cloudContainer, cloud)
end
--
---Variables du curseur---
--
cursor = {
    x = wdowHgt / 2.2,
    y = wdowHgt / 2.2,
    spd = wdowHgt / 100,
    rotation = 0,
    img = image("assets/images/cursor.png")
}
cursor.imgWth = cursor.img:getWidth()
cursor.imgHgt = cursor.img:getHeight()
cursor.wth = wdowHgt / 10 / cursor.imgWth
cursor.hgt = wdowHgt / 10 / cursor.imgHgt
cursor.xShot = (cursor.x + cursor.imgWth * cursor.wth / 2)
cursor.yShot = (cursor.y + cursor.imgHgt * cursor.hgt / 2)
cursor.spdDown = 1
--
---Variables du tableau de bord---
--
panel = {
    x = wdowHgt,
    y = 0,
    img = image("assets/images/panel.png")
}
panel.imgWth = panel.img:getWidth()
panel.imgHgt = panel.img:getHeight()
panel.wth = (wdowWth - wdowHgt) / panel.imgWth
panel.hgt = wdowHgt / panel.imgHgt
--
---Variables pour le classement---
--
classementTable = {}
for i = 0, 9 do
    classement = {}
    classement.score = -1
    classement.name = name
    table.insert(classementTable, classement)
end
--
---Sons---
--
shotSound = sound.newSource("assets/sounds/shot.wav")
cloudSound = sound.newSource("assets/sounds/cloud-die.mp3")
writeSound = sound.newSource("assets/sounds/write.wav")
winSound = sound.newSource("assets/sounds/win.mp3")