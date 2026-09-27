-- MK11 Menu Accessibility Hook
-- MK11Hook Mods klasörüne konulacak Lua scripti
-- Menü olaylarını ekran okuyucu köprüsüne gönderir

local menuState = {
    isMenuOpen = false,
    currentMenuName = "",
    currentSelection = 0,
    menuItems = {},
    lastAnnounced = ""
}

local pipeHandle = nil

-- Adlandırılmış pipe'a bağlanmaya çalış
local function connectToPipe()
    -- Windows API - fopen yerine file I/O kullanacağız
    -- Lua'da pipe bağlantısı sınırlı olduğu için dosya yazma kullanacağız
    local pipeFile = io.open("\\\\.\\pipe\\MK11AccessibilityEvents", "w")
    if pipeFile then
        pipeFile:setvbuf("no") -- Tampon yok, doğrudan yaz
        return pipeFile
    end
    return nil
end

-- Olayı pipe'a gönder
local function sendEvent(eventType, data)
    if not pipeHandle then
        pipeHandle = connectToPipe()
    end
    
    if pipeHandle then
        local json = buildJson(eventType, data)
        pipeHandle:write(json .. "\n")
        pipeHandle:flush()
    end
end

-- JSON oluştur (basit implement)
function buildJson(eventType, data)
    local json = "{\"Type\":\"" .. eventType .. "\""
    if data then
        for k, v in pairs(data) do
            if type(v) == "string" then
                json = json .. ",\"" .. k .. "\":\"" .. tostring(v) .. "\""
            elseif type(v) == "number" then
                json = json .. ",\"" .. k .. "\":" .. tostring(v)
            end
        end
    end
    json = json .. "}"
    return json
end

-- Menü açıldığında
local function onMenuOpen(menuName, items)
    menuState.isMenuOpen = true
    menuState.currentMenuName = menuName
    menuState.menuItems = items or {}
    menuState.currentSelection = 1
    
    sendEvent("menu", {
        Text = menuName,
        Items = #items
    })
    
    announceSelection()
end

-- Menü kapandığında
local function onMenuClose()
    menuState.isMenuOpen = false
    sendEvent("menu_close", {})
end

-- Seçim değiştiğinde
local function onSelectionChange(index)
    menuState.currentSelection = index
    announceSelection()
end

-- Seçimi seslendir
function announceSelection()
    if menuState.currentSelection > 0 and menuState.currentSelection <= #menuState.menuItems then
        local item = menuState.menuItems[menuState.currentSelection]
        local text = tostring(item.name or item) .. ". " .. (item.description or "")
        
        if text ~= menuState.lastAnnounced then
            sendEvent("menu_select", {
                Text = text,
                Index = menuState.currentSelection,
                Total = #menuState.menuItems
            })
            menuState.lastAnnounced = text
        end
    end
end

-- Tur başladı
local function onRoundStart(roundNumber)
    sendEvent("round_start", {
        Round = roundNumber
    })
end

-- Can durumu güncellendi
local function onHealthUpdate(p1Health, p1Max, p2Health, p2Max)
    sendEvent("health", {
        Player1Health = math.floor((p1Health / p1Max) * 100),
        Player2Health = math.floor((p2Health / p2Max) * 100)
    })
end

-- Kombo başladı
local function onComboStart(player, comboCount)
    if player == 1 then
        menuState.p1ComboCount = comboCount
    else
        menuState.p2ComboCount = comboCount
    end
    
    sendEvent("combo", {
        Player1Combo = menuState.p1ComboCount or 0,
        Player2Combo = menuState.p2ComboCount or 0
    })
end

-- Tur bitti
local function onRoundEnd(winner)
    sendEvent("round_end", {
        Text = "Tur bitti. Kazanan: Oyuncu " .. tostring(winner)
    })
end

-- Oyun bitti
local function onFightEnd(winner)
    sendEvent("fight_end", {
        Text = "Maç bitti. Kazanan: Oyuncu " .. tostring(winner)
    })
end

-- Modülü başlat
local AccessibilityModule = {}

function AccessibilityModule.initialize()
    pipeHandle = connectToPipe()
    if pipeHandle then
        sendEvent("system", { Text = "MK11 Erişilebilirlik Modülü Başlatıldı" })
        return true
    end
    return false
end

function AccessibilityModule.shutdown()
    if pipeHandle then
        pipeHandle:close()
        pipeHandle = nil
    end
end

function AccessibilityModule.onMenuOpen(name, items)
    onMenuOpen(name, items)
end

function AccessibilityModule.onMenuClose()
    onMenuClose()
end

function AccessibilityModule.onSelectionChange(index)
    onSelectionChange(index)
end

function AccessibilityModule.onRoundStart(round)
    onRoundStart(round)
end

function AccessibilityModule.onHealthUpdate(p1h, p1m, p2h, p2m)
    onHealthUpdate(p1h, p1m, p2h, p2m)
end

function AccessibilityModule.onComboStart(player, count)
    onComboStart(player, count)
end

function AccessibilityModule.onRoundEnd(winner)
    onRoundEnd(winner)
end

function AccessibilityModule.onFightEnd(winner)
    onFightEnd(winner)
end

-- Modülü başlat
AccessibilityModule.initialize()

return AccessibilityModule
