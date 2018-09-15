--[[
    Titre    : Cloud Hunt
    Créateur : Rui Manuel Mota Carneiro
    Aide     : Leandro Saraiva Maia (code) / Brian Rodrigues Fraga (musique)
    Date     : 11.11.2017 à 17.11.2017
    Version  : Beta v1.0
]]--
--
---Données---
--
function love.load()
--Appelle les paramètres
    require "setup"
--Appelle des fonctions
    require "functions"
end
--
---Traitement des données---
--
function love.update(dt)
--
--Exécute cette partie du code si le jeu est déclaré comme lancé et qu'il n'y a pas encore de victoire
    if start and not win then
    --Incrémente le temps
        time = time + dt 
    --Exécute cette partie du code si le temps est supérieur à la variable "frame"
        if time > frame then
        --Cette portion de code s'éxecute donc 50 fois par secondes
            frame = frame + timeFrame
        --Etabli le temps restant pour continuer le combo
            if timeCombo > 0 then timeCombo = timeCombo - 0.02 elseif timeCombo < 0 then timeCombo = 0 end
            
        --Si tous les nuages ont été tués, on déclare la victoire comme étant vraie et que le tableau des scores doit être mis à jour
            if kills == killsLimit then
                win = true
                scoreTableRefresh = true
            end
            
        --Lance toutes les fonctions d'animations
            backAnime()
            cloudAnime()
            --Animation de disparission du nuage
            cloudTouched()
        end
        
    --Gère les contrôles du curseur
        controls()
        
    --Déclare que le jeu ne doit pas être mis en pause
        pause = false
        
--Exécute cette partie du code si le jeu n'est pas lancé ou qu'il y a une victoire / mode : pause
    else
    --Si la demande de nom est nécessaire
        if enterName then
        --Demande du nom
            playerName()
    --Si la demande du nom n'est pas nécessaire
        else
        --Déclare que le jeu doit rentrer en mode pause
            pause = true
        --Affiche le tableau des scores
            showScoreTable()
        --Quitte le programme en cas d'appui sur la touche "escape/Esc"
            if keyDown("escape") then os.exit() end
        --En cas d'appui sur une touche :
            function love.keypressed(key)
            --Si cette touche correspond aux touches indiquées :
                if key == "return" or key == "kpenter" or key == "space" then
                --Relance le jeu
                    refreshAll()
                end
            end
        end
    end
end
--
---Affichage---
--
function love.draw()
--Affichage de l'arrière plan
    draw(back.img, back.x, back.y, _, back.wth, back.hgt)
--Affichage des nuages, 1 par 1
    for i, cloud in pairs(cloudContainer) do
    --Change la couleur des images à afficher
        color(255, 255, 255, cloud.opacity)
    --Affiche le nuage
        draw(cloudImg, cloud.x, cloud.y, _, cloud.scale, cloud.scale)
    end
    color(255, 255, 255, 255)--reset des couleurs
--Affichage du curseur
    draw(cursor.img, cursor.x, cursor.y, _, cursor.wth, cursor.hgt)
--Affichage de l'écran du mode pause
    color(0, 0, 0, 180)
    if pause then--Si le mode pause est activé
    --Dessine un réctangle transparent recouvrant la zone de jeu
        love.graphics.rectangle("fill", 0, 0, wdowWth, wdowHgt) 
        color(255, 255, 255, 255)--reset des couleurs
        lg.print("Classement", textSize * 36, textSize * 9, _, textSize * 1.5, textSize * 1.5)
        for i, class in pairs(classementTable) do
        --Affichage de l'index et du nom du joueur dans le classement
            lg.print(string.format("%2.2d   %s", i, class.name), textSize * 18, textSize * 18 *  (i + 1), _, textSize * 1.5, textSize * 1.5)
        --Si il n'y a pas assez de joueur dans le classement, affiche des scores de 0 et aucun noms pour les dernières places
            if class.score == -1 then
                lg.print(string.format("%5.1d", 0), textSize * 126, textSize * 18 * (i + 1), _, textSize * 1.5, textSize * 1.5)
        --Affiche le score du joueur
            else
                lg.print(string.format("%5.1d", class.score), textSize * 126, textSize * 18 * (i + 1), _, textSize * 1.5, textSize * 1.5)
            end
        end
    end
    color(255, 255, 255, 255)--reset des couleurs
--Affichage du tableau de bord
    draw(panel.img, panel.x, panel.y, _, panel.wth, panel.hgt)
--Affichage d'informations
    lg.print(string.format("Temps : %d", time), wdowHgt + textSize * 12, textSize * 12, _, textSize, textSize)
    --lg.print("TempsC : " .. timeCombo, wdowHgt + textSize * 12, textSize * 24, _, textSize, textSize)
    lg.print("Tirs        : " .. shots, wdowHgt + textSize * 12, textSize * 48, _, textSize, textSize)
    lg.print("Ratés     : " .. fails, wdowHgt + textSize * 12, textSize * 60, _, textSize, textSize)
    lg.print("Touchés : " .. kills, wdowHgt + textSize * 12, textSize * 72, _, textSize, textSize)
    lg.print("Combos  : " .. comboPts, wdowHgt + textSize * 12, textSize * 84, _, textSize, textSize)
    lg.print("Score : " .. score, wdowHgt + textSize * 12, textSize * 108, _, textSize, textSize)
--Affichage du mot Victoire! si le jeu est gagné
    if win and not pause then
        color(0, 0, 0)
        lg.print("Victoire!", wdowHgt / 2 - textSize * 72, wdowHgt / 2 - textSize * 36, _, textSize * 3, textSize * 3)
        lg.print("Ecrivez votre nom.", wdowHgt / 2 - textSize * 60, wdowHgt / 2, _, textSize, textSize)
        color(255,255,255)
    end
--Affichage du nom
    lg.print("Nom :", wdowHgt + textSize * 16, textSize * 196, _, textSize, textSize)
    lg.print(name, wdowHgt + textSize * 24, textSize * 210, _, textSize * 2, textSize * 2)
end