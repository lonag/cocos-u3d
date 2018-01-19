

local MainScene = class("MainScene", function()
    return display.newScene("MainScene")
end)

function MainScene:ctor()
    self._scale = nil
    self._size = nil
    self._scrollView = nil
    self._image = nil
    self._color_blocks = {}
    self._colors = {}
end

function MainScene:onEnter()
    self:createView()
    self:updateView()
    self:updateColorView()
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
   
    local scale = 20
    local image = cc.Image:new()
    image:retain()
    image:initWithImageFile("00862-or8.png")
    local size = cc.size(image:getWidth(), image:getHeight())

    self._scale = scale
    self._size = size

    self._image = image

    scrollView:addTouchEventListener(function (sender, event)
        if event == ccui.TouchEventType.began then
            -- self:checkBlock(cc.p(sender:getTouchBeganPosition()))
        elseif event == ccui.TouchEventType.moved then

        elseif event == ccui.TouchEventType.ended or event == ccui.TouchEventType.canceled then

        end
    end)
end

function MainScene:updateView()
    local size = self._size
    local image = self._image
    local scale = self._scale
    
    local x, y = display.width/2 - size.width/2*scale, display.height - size.height*scale/2

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
            local fillColor = cc.c4b(gray/255, gray/255, gray/255, 255/255)
            if not (color.r == 255 and color.g == 255 and color.b == 255) then
                if self._color_blocks[i] and self._color_blocks[i][j] then
                    drawNode:drawPolygon(points, 4, self._color_blocks[i][j], 0.5, cc.c4f(1, 1, 1, 0.2))
                else
                    drawNode:drawPolygon(points, 4, fillColor, 0.5, cc.c4f(1, 1, 1, 0.2))
                end
                local label = display.newTTFLabel({text = self:getColorIndex(color),size = scale/2})
                display.align(label, display.CENTER, (i-0.5)*scale + x, (j-0.5)*scale + y)
                self._scrollView:addChild(label)
            end
        end
    end

end

function MainScene:updateColorView()
    local bottom = ccui.Layout:create()
    bottom:setBackGroundColorType(1)
    bottom:setBackGroundColor(cc.c3b(255, 255, 255))
    bottom:setContentSize(cc.size(display.width, 100))
    display.align(bottom, display.CENTER, display.width/2, 50)
    self:addChild(bottom)

    local scrollView = ccui.ScrollView:create()
    self._scrollView = scrollView
    self:addChild(scrollView)
    scrollView:setDirection(ccui.ScrollViewDir.horizontal)
    scrollView:setContentSize(cc.size(display.size.width, 50))
    scrollView:setInnerContainerSize(cc.size(display.size.width*2, 50))
    scrollView:setAnchorPoint(cc.p(0.5, 0.5))
    scrollView:setPosition(cc.p(display.size.width/2, 50))
    scrollView:setTouchEnabled(true)

    for i, v in pairs(self._colors) do
        local layout = ccui.Layout:create()
        layout:setBackGroundColorType(1)
        layout:setBackGroundColor(v)

        layout:setContentSize(cc.size(50, 50))
        display.align(layout, display.CENTER, i*60, 25)
        scrollView:addChild(layout)

        local label = display.newTTFLabel({text = i, size = 15})
        display.align(label, display.CENTER, i*60, 25)
        scrollView:addChild(label)
    end

end

function MainScene:getColorIndex(color)
    for i, v in pairs(self._colors) do
        if v.r == color.r and v.g == color.g and v.b == color.b then
            return i
        end
    end

    table.insert(self._colors, color)
    return table.nums(self._colors)
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
    if self._image then
        self._image:release()
    end
end

return MainScene

int lua_cocos2dx_Image_getColorAt(lua_State* tolua_S)
{
    int argc = 0;
    cocos2d::Image* cobj = nullptr;
    
#if COCOS2D_DEBUG >= 1
    tolua_Error tolua_err;
#endif
    
#if COCOS2D_DEBUG >= 1
    if (!tolua_isusertype(tolua_S,1,"cc.Image",0,&tolua_err)) goto tolua_lerror;
#endif
    
    cobj = (cocos2d::Image*)tolua_tousertype(tolua_S,1,0);
    
#if COCOS2D_DEBUG >= 1
    if (!cobj)
    {
        tolua_error(tolua_S,"invalid 'cobj' in function 'lua_cocos2dx_Image_getColorAt'", nullptr);
        return 0;
    }
#endif
    
    argc = lua_gettop(tolua_S)-1;
    if (argc == 2)
    {
#if COCOS2D_DEBUG >= 1
		int bitPerPixel = cobj->getBitPerPixel();
        if (bitPerPixel != 32) {
            //tolua_error(tolua_S,"cc.Image:getColorAt ONLY work for Image 8888", nullptr);
            //return 0;
        }
#endif
        lua_Integer px = lua_tointeger(tolua_S, 2);
        lua_Integer py = lua_tointeger(tolua_S, 3);
        unsigned char *data = cobj->getData();
        cobj->getWidth();
        /*uint32_t *pixel = (uint32_t *)data;
        pixel += py * cobj->getWidth() + px;*/
        
        Color4B color;
        color.r = data[(py * cobj->getWidth() + px) * 3];
        color.g = data[(py * cobj->getWidth() + px) * 3 + 1]; //(*pixel >> 8) & 0xff;
        color.b = data[(py * cobj->getWidth() + px) * 3 + 2]; //(*pixel >> 16) & 0xff;
		//color.a = (*pixel >> 24) & 0xff;

        color4b_to_luaval(tolua_S, color);
        return 1;
    }
    luaL_error(tolua_S, "%s has wrong number of arguments: %d, was expecting %d \n", "cc.Image:getColorAt",argc, 2);
    return 0;
    
#if COCOS2D_DEBUG >= 1
tolua_lerror:
    tolua_error(tolua_S,"#ferror in function 'lua_cocos2dx_Image_getColorAt'.",&tolua_err);
#endif
    
    return 0;
}

int lua_cocos2dx_Image_setColorAt(lua_State* tolua_S)
{
	int argc = 0;
	cocos2d::Image* cobj = nullptr;

#if COCOS2D_DEBUG >= 1
	tolua_Error tolua_err;
#endif

#if COCOS2D_DEBUG >= 1
	if (!tolua_isusertype(tolua_S, 1, "cc.Image", 0, &tolua_err)) goto tolua_lerror;
#endif

	cobj = (cocos2d::Image*)tolua_tousertype(tolua_S, 1, 0);

#if COCOS2D_DEBUG >= 1
	if (!cobj)
	{
		tolua_error(tolua_S, "invalid 'cobj' in function 'lua_cocos2dx_Image_setColorAt'", nullptr);
		return 0;
	}
#endif

	argc = lua_gettop(tolua_S) - 1;
	if (argc == 3)
	{
#if COCOS2D_DEBUG >= 1
		int bitPerPixel = cobj->getBitPerPixel();
		if (bitPerPixel != 32) {
			tolua_error(tolua_S, "cc.Image:setColorAt ONLY work for Image 8888", nullptr);
			return 0;
		}
#endif
		lua_Integer px = lua_tointeger(tolua_S, 2);
		lua_Integer py = lua_tointeger(tolua_S, 3);

		Color4F color;
		if (!luaval_to_color4f(tolua_S, 4, &color, "cc.Image:setColorAt"))
		{
			return 0;
		}

		unsigned char *data = cobj->getData();
		cobj->getWidth();
		uint32_t *pixel = (uint32_t *)data;
		pixel[py * cobj->getWidth() + px] = color.r;
		pixel[py * cobj->getWidth() + px + 1] = color.g;
		pixel[py * cobj->getWidth() + px + 2] = color.b;
		pixel[py * cobj->getWidth() + px + 3] = color.a;

		return 1;
	}
	luaL_error(tolua_S, "%s has wrong number of arguments: %d, was expecting %d \n", "cc.Image:setColorAt", argc, 2);
	return 0;

#if COCOS2D_DEBUG >= 1
	tolua_lerror:
				tolua_error(tolua_S, "#ferror in function 'lua_cocos2dx_Image_setColorAt'.", &tolua_err);
#endif

	return 0;
}