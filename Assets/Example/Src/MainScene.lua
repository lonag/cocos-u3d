
local MainScene = class("MainScene", function()
    return display.newScene("MainScene")
end)

function MainScene:ctor()
    self._scale = nil
    self._size = nil
    self._scrollView = nil
    self._image = nil
end

function MainScene:onEnter()
    self:updateView()
end

function MainScene:updateView()
    local scrollView = ccui.ScrollView:create()
    scrollView:setDirection(ccui.ScrollViewDir.both)
    scrollView:setContentSize(display.size)
    scrollView:setInnerContainerSize(cc.size(display.size.width*2, display.height*2))
    scrollView:setAnchorPoint(cc.p(0.5, 0.5))
    scrollView:setPosition(cc.p(display.size.width/2, display.height/2))
    scrollView:setTouchEnabled(true)
    self:addChild(scrollView)
    
    self._scrollView = scrollView

    scrollView:addTouchEventListener(function (sender, event)
        if event == ccui.TouchEventType.began then
            self:checkBlock(cc.p(sender:getTouchBeganPosition()))
        elseif event == ccui.TouchEventType.moved then

        elseif event == ccui.TouchEventType.ended or event == ccui.TouchEventType.canceled then

        end
    end)

    local scale = 20
    local image = cc.Image:new()
    image:initWithImageFile("jifen.png")
    local size = cc.size(image:getWidth(), image:getHeight())
    local x, y = display.width/2 - size.width/2*scale, display.height/2 - size.height*scale/2
    
    local drawNode = cc.DrawNode:create()
    drawNode:setName("drawNode")
    scrollView:addChild(drawNode)
    
    for i = 1, size.width do
        for j = 1, size.height do
            local points = {
                cc.p((i-1)*scale + x, (j-1)*scale + y),
                cc.p((i-1)*scale + x, j*scale + y),
                cc.p(i*scale + x, j*scale + y),
                cc.p(i*scale + x, (j-1)*scale + y),
            }   
            local color = image:getColorAt(i - 1, size.height - j)
            local gray = color.r*0.299 + color.g*0.587 + color.b*0.114
            -- local gray = (r*299 + g*587 + b*114 + 500) / 1000
            -- local gray = (r*30 + g*59 + b*11 + 50) / 100
            local fillColor = cc.c4b(gray/255, gray/255, gray/255, color.a/255)
            if not (color.r == 0 and color.g == 0 and color.b == 0) then
                drawNode:drawPolygon(points, 4, fillColor, 0.5, cc.c4f(1, 1, 1, 0.2))
            end
        end
    end
    self._scale = scale
    self._size = size

    self._image = image
end

function MainScene:checkBlock(point)
    local x,y = display.width/2 - self._size.width/2*self._scale, display.height/2 - self._size.height*self._scale/2
    local container = self._scrollView:getInnerContainer()

    x = x + container:getPositionX()
    y = y + container:getPositionY()
    
    print(container:getPositionX(), container:getPositionY())

    local rect = cc.rect(x, y, self._scale*30, self._scale*30)
    if cc.rectContainsPoint( rect, point ) then
        local px = math.floor((point.x - rect.x)/self._scale)
        local py = math.floor((point.y - rect.y)/self._scale)
        print("px,py",px, py)
    end
end

function MainScene:onExit()

end

return MainScene
