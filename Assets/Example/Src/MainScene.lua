
local MainScene = class("MainScene", function()
    return display.newScene("MainScene")
end)

function MainScene:ctor()
    self._scale = nil
    self._size = nil
    self._scrollView = nil
    self._image = nil
    self._color_blocks = {}
end

function MainScene:onEnter()
    self:createView()
    self:updateView()
end

function MainScene:createView()
    local scrollView = ccui.ScrollView:create()
    self._scrollView = scrollView
    self:addChild(scrollView)
    scrollView:setDirection(ccui.ScrollViewDir.both)
    scrollView:setContentSize(display.size)
    scrollView:setInnerContainerSize(cc.size(display.size.width*2, display.height*2))
    scrollView:setAnchorPoint(cc.p(0.5, 0.5))
    scrollView:setPosition(cc.p(display.size.width/2, display.height/2))
    scrollView:setTouchEnabled(true)
   
    local scale = 5
    local image = cc.Image:new()
    image:retain()
    image:initWithImageFile("jifen.png")
    local size = cc.size(image:getWidth(), image:getHeight())

    self._scale = scale
    self._size = size

    self._image = image

    scrollView:addTouchEventListener(function (sender, event)
        if event == ccui.TouchEventType.began then
            self:checkBlock(cc.p(sender:getTouchBeganPosition()))
        elseif event == ccui.TouchEventType.moved then

        elseif event == ccui.TouchEventType.ended or event == ccui.TouchEventType.canceled then

        end
    end)
end

function MainScene:updateView()
    local size = self._size
    local image = self._image
    local scale = self._scale

    
    local x, y = display.width/2 - size.width/2*scale, display.height/2 - size.height*scale/2

    local drawNode = self._scrollView:getChildByName("drawNode")
    if not drawNode then
        drawNode = cc.DrawNode:create()
        drawNode:setName("drawNode")
        self._scrollView:addChild(drawNode)
    end

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
                if self._color_blocks[i] and self._color_blocks[i][j] then
                    drawNode:drawPolygon(points, 4, self._color_blocks[i][j], 0.5, cc.c4f(1, 1, 1, 0.2))
                else
                    drawNode:drawPolygon(points, 4, fillColor, 0.5, cc.c4f(1, 1, 1, 0.2))
                end
            end
        end
    end

end

function MainScene:checkBlock(point)
    local x,y = display.width/2 - self._size.width/2*self._scale, display.height/2 - self._size.height*self._scale/2
    local container = self._scrollView:getInnerContainer()

    x = x + container:getPositionX()
    y = y + container:getPositionY()
    
    print(container:getPositionX(), container:getPositionY())

    local rect = cc.rect(x, y, self._scale*30, self._scale*30)
    if cc.rectContainsPoint( rect, point ) then
        local px = math.floor((point.x - rect.x)/self._scale) + 1
        local py = math.floor((point.y - rect.y)/self._scale) + 1
        print("px,py",px, py)
        if not self._color_blocks[px] then
            self._color_blocks[px] = {}
        end
        if not self._color_blocks[px][py] then
            self._color_blocks[px][py] = cc.c4b(1, 0, 0, 1)
        end
        -- self._image:setColorAt(px, py, cc.c4b(255, 0, 0, 255))
        self:updateView()
    end
end

function MainScene:onExit()

end

return MainScene
