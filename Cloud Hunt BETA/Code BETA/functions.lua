--
---Animation du fond
--
function backAnime()
--Change le sens de l'animation : haut/bas
    if back.y < (0 - (wdowHgt / 5)) or back.y > 0 then
        back.verticalAnime = 3 - back.verticalAnime
    end
--Vers le bas
    if back.verticalAnime == 1 then
        back.y = back.y + back.spd
--Vers le haut
    elseif back.verticalAnime == 2 then
        back.y = back.y - back.spd
--Aucun des deux
    else
        back.y = back.y
    end
end
--
---Déplacement des nuages
--
function cloudAnime()
--Prend chaque nuage
    for i, cloud in pairs(cloudContainer) do
    --Déplace le nuage à droite ou à gauche
        cloud.x = cloud.x + (cloud.spd * ((cloud.direction * 2) - 1))
    --Si le nuage sort de l'écran, le fait réapparaître ailleurs
        if cloud.x < -100 or cloud.x > (wdowHgt + 100) then
        --Choisi la direction du nuage : droite/gauche
            cloud.direction = math.random(0, 1)
        --Choisi le lieu d'apparition du nuage
            cloud.x = math.random(wdowHgt, wdowHgt + cloudImgWth * maxCloudSize / 100)
            cloud.x = cloud.x - ((cloud.x + cloudImgWth * maxCloudSize / 100) * cloud.direction)
        --Choisi la hauteur à la quelle le nuage apparaît
            cloud.y = math.random(0, wdowHgt - (cloudImgHgt * 3))
        end
    end
end
--
---Fait disparaitre un nuage
--
function cloudTouched()
--Active la vérification pour savoir si on a gagné
    winVerif = true
--Prend chaque nuage
    for i, cloud in pairs(cloudContainer) do
    --Si le nuage est touché mais qu'il n'a pas disparu, le fait disparaître petit à petit
        if cloud.isTouched and cloud.opacity ~= 0 then
            cloud.opacity = cloud.opacity - opacityLevel
        end
    --Si tous les nuages n'ont pas disparu, il ne peut pas y avoir de victoire
        if cloud.opacity ~= 0 then
        --Il n'y a donc pas de victoire
            win = false
        end
    end
--Si il y a une victoire, la demande de nom est activée
    if win then
        enterName = true
    end
end
--
---Tir
--
function shot()
--Déclaration de la variable de vérification du tir raté
    local failVerif = false
--Prend chaque nuage
    for i, cloud in pairs(cloudContainer) do
    --Vérifie que le viseur est sur le nuage et qu'il n'a pas été touché
        if (cursor.xShot < cloud.x + cloudImgWth * cloud.scale and cursor.xShot > cloud.x)
        and (cursor.yShot < cloud.y + cloudImgHgt * cloud.scale and cursor.yShot > cloud.y) 
        and not cloud.isTouched then
        --Le nuage est touché
            cloud.isTouched = true
        --Il n'y a pas de tir raté
            failVerif = true
        --Incrémentation des nuages touchés
            kills = kills + 1
        --Joue le son d'un nuage touché
            cloudSound:play()
        --Incrémente comboPts(30 pts) si timeCombo est supérieur à 0
            if timeCombo > 0 then
                comboPts = comboPts + 1
            end
        --Place timeCombo à 1
            timeCombo = 1
        end
    end
--Si le tir est raté
    if not failVerif then
    --Incrémente les tirs ratés
    	fails = fails + 1 
    --Place timeCombo à 0 pour arrêter les combos
        timeCombo = 0
    --Joue le son d'un tir
        shotSound:play()
    end
--Incrémente les tirs
    shots = shots + 1
--Calcul du score
    refreshScore()
end
--
---Gère les contrôles du viseur
--
function controls()
--Diminue la vitesse par 2 quand "shift|Maj" est appuyé
    if keyDown("lshift") or keyDown("rshift") then cursor.spdDown = 2 else cursor.spdDown = 1 end
--Déplace le curseur à gauche
    if (keyDown("left") or keyDown("a")) and cursor.xShot > 0 then
        cursor.x = cursor.x - (cursor.spd / cursor.spdDown)
        cursor.xShot = cursor.xShot - (cursor.spd / cursor.spdDown)
    end
--Déplace le curseur à droite
    if (keyDown("right") or keyDown("d")) and cursor.xShot < wdowHgt then
        cursor.x = cursor.x + (cursor.spd / cursor.spdDown)
        cursor.xShot = cursor.xShot + (cursor.spd / cursor.spdDown)
    end
--Déplace le curseur en haut
    if (keyDown("up") or keyDown("w")) and cursor.yShot > 0 then
        cursor.y = cursor.y - (cursor.spd / cursor.spdDown)
        cursor.yShot = cursor.yShot - (cursor.spd / cursor.spdDown)
    end
--Déplace le curseur en bas
    if (keyDown("down") or keyDown("s")) and cursor.yShot < wdowHgt then
        cursor.y = cursor.y + (cursor.spd / cursor.spdDown)
        cursor.yShot = cursor.yShot + (cursor.spd / cursor.spdDown)
    end
--Si une touche est appuyé
    function love.keypressed(key)
    --Tire si on appuie sur les touches suivantes
        if key == "return" or key == "kpenter" or key == "space" then
            shot()
        end
    --Quitte l'application si on appuie sur "Esc"
        if key == "escape" then
            os.exit()
        end
    end
end
--
---Calcul du score
--
function refreshScore()
    score = 5 * kills - 5 * fails - math.floor(time)
--Multi-kill
    if kills > shots - fails then
        score = score + (kills + fails - shots) * 20
    end
--Combos
    if comboPts > 0 then
        score = score + comboPts * 30
    end
--Le score ne peut pas être négatif
    if score < 0 then
        score = 0
    end
end
--
---Rejouer / Recommencer
--
function refreshAll()
--Replace toutes les variables à leurs valeurs de base
    start = true
    win = false
    time = 0
    frame = 0
    timeCombo = 1
    shots = 0
    fails = 0
    kills = 0
    score = 0
    comboPts = 0
    for i, cloud in pairs(cloudContainer) do
        cloud.x = math.random(0 - (cloudImgWth * maxCloudSize / 100), wdowHgt + (cloudImgWth * maxCloudSize / 100))
        cloud.y = math.random(0, wdowHgt - (cloudImgHgt * 3))
        cloud.scale = math.random(minCloudSize, maxCloudSize) / 100
        cloud.spd = 4 - cloud.scale
        cloud.direction = math.random(0, 1)
        cloud.opacity = 255
        cloud.isTouched = false
    end
    cursor.x = wdowHgt / 2.2
    cursor.y = wdowHgt / 2.2
    cursor.xShot = (cursor.x + cursor.imgWth * cursor.wth / 2)
    cursor.yShot = (cursor.y + cursor.imgHgt * cursor.hgt / 2)
end
--
---Demande le nom du joueur
--
function playerName()
--si le nom du joueur est nécessaire
    if enterName then
    --Vérifie quelle touche est pressée
        function love.keypressed(key)  
        --Si la touche est "backspace", supprimme le dernier caractère
            if key == "backspace" then
            --Vérifie que le nom contient au moins 1 caractère
                if string.len(name) > 0 then
                    name = string.sub(name, 0, string.len(name) - 1)
                --Joue le son d'écriture
                    writeSound:play()
                end
        --Vérifie que le touche appuyée ne contient qu'un seul caractère et que le nom ne contient pas encore 3 caractères
            elseif string.len(key) == 1 and string.len(name) < 3 then
            --Ecrit le caractère(en majuscule)
                name = name .. string.upper(key)
            --Joue le son d'écriture
                writeSound:play()
        --Vérifie que le nom contient 3 caractères et le valide quand on appuie sur les touches suivantes
            elseif (key == "return" or key == "kpenter" or key == "space") and string.len(name) == 3 then
            --Déclare qu'il n'y a plus besoin d'entrer le nom
                enterName = false
            --Ecrit dans le fichier des scores
                writeScoreTable()
            --Reset l'affichage du nom
                name = ""
            --Joue le son de victoire/affichage du classement
                winSound:play()
            end
        end
    end
end
--
---Lecture du fichier des scores
--
function readScoreTable()
--Récupération du fichier en lecture
    local file = io.open("data/scores.txt", "r")
--Récupére tous les caractères du fichier
    scoreText = file:read("*all")
--Ferme le fichier
    file:close()
end
--
---Ecriture dans le fichier des scores
--
function writeScoreTable()
--Récupération du fichier en lecture
    local file = io.open("data/scores.txt", "r")
--Récupération de tous les caractères du fichier
    fileText = file:read("*all")
--Fermeture du fichier
    file:close()
--Ouverture du fichier en écriture
    file = io.open("data/scores.txt", "w")
--Ecriture du fichier
    file:write(fileText .. score .. " " .. name .. "\n")
--Fermeture du fichier
    file:close()
end
--
---Affichage du tableau des scores
--
function showScoreTable()
--Si le jeu a besoin de récupérer les score, lis le fichier des scores
    if scoreTableRefresh then readScoreTable() end
--Variable permettant de séparer le score du nom
    local space = 0
--Variable permettant de séparer les différents noms et scores
    local eol = -1
--Variables comptant le nombre de scores et noms
    local num = 0
--Table contenant les noms
    local name = {}
--Table contenant les scores
    local score = {}
--Lis tout le fichier, lettre par lettre
    for i = 0, string.len(scoreText) do
    --Si la caractère trouvé est un espace, récupére le score
        if string.sub(scoreText, i, i) == " " then
            space = i
            score[num] = tonumber(string.sub(scoreText, eol + 1, space - 1))
        end
    --Si le caractère trouvé est un retour à la ligne, récupére le nom
        if string.sub(scoreText, i, i) == "\n" then
            eol = i
            name[num] = string.sub(scoreText, space + 1, eol - 1)
        --Augmente le nombre de scores existants
            num = num + 1
        end
    end
--Rempli le classement
    if scoreTableRefresh then
    --Regarde chaque emplacement du classement
        for i, class in pairs(classementTable) do
        --Compare le nombre de scores existants
            for j = 0, num - 1 do
            --Si le score est supérieur au score dans le classement, le remplace
                if score[j] > class.score then
                    class.name = name[j]
                    class.score = score[j]
                end
            end
        --Si le score & nom est déjà existant dans le classement, le remet à zéro
            for j = 0, num - 1 do
                if score[j] == class.score and name[j] == class.name then score[j] = -1 end
            end
        end
    --Termine la mise à jour du classement
        scoreTableRefresh = false
    end
end