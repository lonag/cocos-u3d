local ViewBase = require "framework.ViewBase"
local GameView = class("GameView", ViewBase)

-- "unblock100X200_lod.png", -- 2
-- "unblock100X300_old.png", -- 5
-- "unblock200X100_old.png",-- 3
-- "unblock300X100_old.png",-- 4
-- "unblock100X300.png",
-- "unblock200X1002.png",-- 1 red

local BLOCK_RES = {
	block1 = "unblock200X1002.png",
	block2 = "unblock100X200_lod.png",
	block3 = "unblock200X100_old.png",
	block4 = "unblock300X100_old.png",
	block5 = "unblock100X300_old.png",
	block6 = ".png",
	block7 = ".png",
}

-- 0 水平 1 垂直
local BLOCK_DIR = {
	block1 = 0,
	block2 = 1,
	block3 = 0,
	block4 = 0,
	block5 = 1,
	block6 = 0,
	block7 = 0,
}

local view_offset_x = 15
local view_offset_y = -15

function GameView:ctor(...)
	GameView.super.ctor(self,...)
	self._blocks = {}
	self._blocks_map_size = 4
	self._blocks_map = nil

	self._widgets = {
		home_btn = false,
		level_title = false,
	}

	display.addSpriteFrames("unblock/unblock.plist", "unblock/unblock.png")

	self._unblock_bg = nil
end

function GameView:initView()
	self:updateView()
	self:createGameView()

	self:loadUI("GameView.csb")
	self:createBgView()

	self._widgets.home_btn._callBack = function()
		self:removeFromParent()
	end
	BaseMethod.setTouchFunc(self._widgets.home_btn)


	self._widgets.level_title:setString(string.format("%s-%d",gGame:getModeName(), gGame:getLevel() - (gGame:getMode()- 1)*50))
end

function GameView:onEnter()
	GameView.super.onEnter(self)

end

function GameView:onExit()
	GameView.super.onEnter(self)
end

function GameView:createBgView()
	local bottom_layout = ccui.Layout:create()
	bottom_layout:setBackGroundColor(cc.c3b(255, 255, 255))
	display.align(bottom_layout,display.CENTER, display.width/2, 40)
	bottom_layout:setContentSize(cc.size(display.width - 50, 80))
	bottom_layout:setBackGroundColorType(1)
	self:addChild(bottom_layout)
	local size = bottom_layout:getContentSize()
	local scale = 1

	local level_label = display.newTTFLabel({
	    text = "愉快的玩游戏吧",
	    font = "Arial",
	    size = 20,
	    color = cc.c3b(225, 208, 192),
	    align = cc.TEXT_ALIGNMENT_CENTER,
	    valign = cc.VERTICAL_TEXT_ALIGNMENT_CENTER,
	    dimensions = cc.size(size.width*scale, size.height*scale)
	})
	display.align(level_label, display.CENTER, size.width/2*scale, size.height/2*scale)
	bottom_layout:addChild(level_label)
end

function GameView:createGameView()
    local unblock_bg4 = ccui.ImageView:create("unblock_bg3.png", 1)
	display.align(unblock_bg4, display.CENTER, display.width/2, display.height/2)
	unblock_bg4:setColor(cc.c3b(225, 214, 185))
	local bgSize = unblock_bg4:getContentSize()
	local offset = 10
    for i=1, 4 do
    	local bg = ccui.Layout:create()
		bg:setBackGroundColor(cc.c3b(245, 244, 244))
		bg:setBackGroundColorType(1)
		if i == 1 then
			display.align(bg,display.CENTER_LEFT, 0, display.height/2)
			bg:setContentSize(cc.size(display.width/2 - bgSize.width/2 + offset, display.height))
		elseif i == 2 then
			display.align(bg,display.TOP_CENTER, display.width/2, display.height)
			bg:setContentSize(cc.size(display.width, display.height/2 - bgSize.height/2 + offset))
		elseif i == 3 then
			bg:setContentSize(cc.size(display.width/2 - bgSize.width/2 + offset, display.height))
			display.align(bg,display.CENTER_RIGHT, display.width, display.height/2)
		elseif i == 4 then
			bg:setContentSize(cc.size(display.width, display.height/2 - bgSize.height/2 + offset))
			display.align(bg,display.BOTTOM_CENTER, display.width/2, 0)
		end

		self:addChild(bg)
    end

    self:addChild(unblock_bg4)
    local scale = 4/self._blocks_map_size
    local offset_y = 18
    if self._blocks_map_size == 4 then
    	offset_y = 18
    elseif self._blocks_map_size == 5 then
    	offset_y = 57
    elseif self._blocks_map_size == 6 then
    	offset_y = 25
    end

	local unblock_bg1= ccui.ImageView:create("unblock_bg2_1.png", 1)
	display.align(unblock_bg1, display.CENTER_LEFT, display.width/2 + bgSize.width/2 - 12, display.height/2 -offset_y + unblock_bg1:getContentSize().height/2)
	unblock_bg1:setColor(cc.c3b(225, 214, 185))
	unblock_bg1:scale(1.2 - (1 - scale))
	self:addChild(unblock_bg1)

	local unblock_bg2 = ccui.ImageView:create("unblock_bg2_2.png", 1)
	unblock_bg2:setScale(scale)
	display.align(unblock_bg2, display.CENTER_LEFT, display.width/2 + bgSize.width/2 - 12, display.height/2 - offset_y + unblock_bg1:getContentSize().height/2)
	unblock_bg2:setColor(cc.c3b(195, 77, 145))
	self:addChild(unblock_bg2)
end

function GameView:updateView()
	self._blocks_map = {
		{0, 0, 0, 0, 0, 0},
		{0, 0, 0, 0, 0 ,0},
		{0, 0, 0, 0, 0, 0},
		{0, 0, 0, 0, 0, 0},
		{0, 0, 0, 0, 0, 0},
		{0, 0, 0, 0, 0, 0},
	}

	if not self._unblock_bg then
    	self._unblock_bg= display.newSprite("#unblock_bg.png")
    	print("unblock_bg is nil")
    	display.align(self._unblock_bg, display.CENTER, display.width/2, display.height/2)
		self:addChild(self._unblock_bg)
    else
    	self._unblock_bg:removeAllChildren()
    end

	print("unblock_bg", self._unblock_bg)

	local bgSize = self._unblock_bg:getContentSize()
	-- 4 x 4
	local level_data = ConfigLoader.getEscapeData(gGame:getLevel())
	self._blocks_map_size = level_data.layout_size

	for i = self._blocks_map_size+1, 6 do
		for j=1, 6 do
			self._blocks_map[i][j] = 1
		end
	end

	for i = 1, 6 do
		for j = self._blocks_map_size + 1, 6 do
			self._blocks_map[i][j] = 1
		end
	end
	-- 100 210
	local width = (bgSize.width - view_offset_x*2)/self._blocks_map_size -- 支持可配置
	width = self:Integer(width)
	local height =  width --(bgSize.height - view_offset_y*2)/self._blocks_map_size -- 支持可配置
	for i=1, 5 do
		local block = level_data["block"..i]

		for j,v in pairs(block) do
			local block = ccui.ImageView:create(BLOCK_RES["block"..i], 1)
			local scale = 4/self._blocks_map_size
			block:setScale(scale)
			local size  = block:getContentSize()
			size = cc.size(size.width * scale, size.height * scale)
			local dir = BLOCK_DIR["block"..i]
			-- local newPos = nil
			local isCanMove = false
			local oldPos = nil
			local min_pos, max_pos
			block:setTouchEnabled(true)
			block:onTouch(function(event)
				
				if event.name == 'began' then
					oldPos = cc.p(event.target:getPosition())
					min_pos, max_pos = self:getCanMovePos(dir, oldPos ,i, j, width, v.len)
					print("touch began")
					-- print("origin = ", event.target:getPosition())
			  --       print("min", table.tostring(min_pos)) 
			  --       print("max",table.tostring(max_pos))
		        elseif event.name == 'moved' then
		        	if not max_pos then
		        		min_pos, max_pos = self:getCanMovePos(dir, oldPos ,i, j, width, v.len)
		        	end
		        	local newPos = event.target:getTouchMovePosition()
		        	
		        	local is_right = event.target:getTouchMovePosition().x - event.target:getTouchBeganPosition().x > 0
		        	local is_top = event.target:getTouchMovePosition().y - event.target:getTouchBeganPosition().y > 0
			        local parent = event.target:getParent()
			        newPos = parent:convertToNodeSpace(newPos)
		
		        	local nextPos = cc.p(newPos.x - size.width/2, newPos.y + size.height/2)
		        	-- FIX 20171214
			        local x , y = nextPos.x, nextPos.y -- event.target:getPosition()
			        
			        if dir == 0 then
			        	if x < max_pos.x and is_right then
			        		block:setPositionX(nextPos.x)
			        	elseif not is_right and x > min_pos.x then
			        		block:setPositionX(nextPos.x)
			        	end
			        else
			        	if is_top and y < max_pos.y then
			        		block:setPositionY(nextPos.y)
			        	elseif not is_top and y > min_pos.y then
			        		block:setPositionY(nextPos.y)
			        	end
			        end
				elseif event.name == 'cancelled' then
					print("cancelled")
				elseif event.name == 'ended' then
					print("ended")

					self:resetBlockMap(dir, BLOCK_RES["block"..i], i, j, 0) -- 删除旧的记录数据
					local isRed = i == 1
					local endPos = cc.p(event.target:getPosition())
					endPos = cc.p(self:Integer(endPos.x), self:Integer(endPos.y))
					-- 自动移动距离补齐一个格子
					local widget_pos_x, widget_pos_y, isOver = self:blockMoveEnded(dir, oldPos, endPos, BLOCK_RES["block"..i], width ,event.target, v.len, i, j)
					local action1 = cc.MoveTo:create(0.2, cc.p(widget_pos_x, widget_pos_y))
					local action2 = cc.CallFunc:create(function()
						if isRed then
							-- 通关条件
							if isOver then
								print("moveCall")
								self:nextLevel()
							end
						end
						end)
					local action = cc.Sequence:create({action1, action2})
					event.target:runAction(action)
					self:resetBlockMap(dir, BLOCK_RES["block"..i], i, j, 1) -- 设置移动后的记录数据
				
				end
			end)

			display.align(block, display.TOP_LEFT, view_offset_x + v.x*width, v.y*height - view_offset_y)
			self._unblock_bg:addChild(block, 10 -v.y)
			-- print(view_offset_x + v.x*width, v.y*height - view_offset_y)
			self._blocks[BLOCK_RES["block"..i]..j] = {x = v.x, y = v.y, len = v.len}
			if dir == 0 then
				for idx = v.x , v.x + v.len - 1 do
					print(idx+1, v.y, "H")
					self._blocks_map[idx + 1][v.y] = 1
					
				end
			else
				for idx = v.y , v.y - v.len + 1, -1 do
					print(v.x + 1, idx, "V")
					self._blocks_map[v.x + 1][idx] = 1
					
				end
			end
		end
	end
end

function GameView:convertToUI(newPos)
	return self._unblock_bg:convertToNodeSpace(newPos)
end

function GameView:getBlockPos(block_pos, isCeil)
	if isCeil then
		return math.ceil(block_pos)
	else
		return math.floor(block_pos)
	end
end

-- 获取可移动范围
function GameView:getCanMovePos(dir, oldPos ,i, j, block_size, len)
	local block = self._blocks[BLOCK_RES["block"..i]..j]
	local isRed = i == 1
	local is_move = true
	local block_x = block.x
	local block_y = block.y

	if dir == 0 then
		-- 0 1 2 3 4 5
		local max_x = block_x -- 2
		
		for i = block_x + len, self._blocks_map_size - 1 do
			print(i + 1, self._blocks_map[i + 1][block_y], "H")
			if self._blocks_map[i + 1][block_y] == 1 then
				max_x = i - len -- 前一个才是可以通行的，故-1	
				is_move = false	
				break
			end
		end
		print("max_x", max_x)
		if max_x < 0 then
			max_x = 0
		end

		if is_move then
			max_x =  self._blocks_map_size - len
		end

		local max_pos = cc.p(max_x* block_size + view_offset_x, oldPos.y)
		is_move = true
		local min_x = block_x
		for i=0, block_x - 1 do
			if self._blocks_map[i + 1][block_y] == 1 then
				min_x = i + 1 -- 后一个才是可以通行的，故+1
				is_move = false
				break
			end
		end
		if is_move then
			min_x = 0
		end

		local min_pos = cc.p(min_x*block_size + view_offset_x, oldPos.y)
		return min_pos, max_pos
	else
		-- 6
		-- 5
		-- 4
		-- 3
		-- 2
		-- 1
		is_move = true
		local max_y = block_y
		for i = block_y + 1, self._blocks_map_size do
			print(block_x + 1, i, self._blocks_map[block_x + 1][i])
			if self._blocks_map[block_x + 1][i] == 1 then
				max_y = i - 1
				is_move = false
				break
			end
		end
		print("max_y", max_y)
		if is_move then
			max_y = self._blocks_map_size
		end

		local max_pos = cc.p(oldPos.x, max_y*block_size - view_offset_y)

		is_move = true
		local min_y = block_y
		for i = len, block_y - len do 
			if self._blocks_map[block_x + 1][i] == 1 then
				min_y = i - 1
				is_move = false
				break
			end
		end
		if is_move then
			min_y = len
		end

		local min_pos = cc.p(oldPos.x, min_y*block_size - view_offset_y)
		return min_pos, max_pos
	end
end


-- FIXME跳过障碍移动， 需要判断能不能移动
function GameView:blockMoveEnded(dir, oldPos, newPos, block_res_id, block_size, block, len, i, j)
	-- local block = self._blocks[block_res_id]
	local isRed = i == 1
	if dir == 0 then
		local isCeil = false
		isCeil = newPos.x - oldPos.x > 0
		local block_data = self._blocks[BLOCK_RES["block"..i]..j]
		local origin_x = block_data.x
		local block_y = self:getBlockPos((oldPos.y - view_offset_y)/block_size, false)

		-- 0 1 2 3 4 5
		if self._blocks_map[origin_x + 1 +len][block_y] == 1 then
			isCeil = false
		end

		local block_x = self:getBlockPos((newPos.x - view_offset_x)/block_size,isCeil)
		
		
		print("new x", block_x)

		
		-- 调整边界
		if block_x > self._blocks_map_size - 1 then
			block_x = self._blocks_map_size - 1
		end

		if block_x < 0 then
			block_x = 0
		end

		-- 调整边界
		if block_x < 0 then
			block_x = 0
		end
		if block_x +len > self._blocks_map_size - 1 then
			if isRed then
				block_x = self._blocks_map_size
			else
				block_x = self._blocks_map_size - len
			end
		end
		block_data.x = block_x

		print(block_data.x, "move", "H")
		local isOver = false
		if block_x == self._blocks_map_size then
			isOver = true
		end
		return block_x* block_size + view_offset_x, oldPos.y, isOver
	else
		local isCeil = false
		isCeil = newPos.y - oldPos.y > 0

		local block_x = self:getBlockPos((oldPos.x - view_offset_x)/block_size, false)
		local block_y = self:getBlockPos((newPos.y - view_offset_y)/block_size, isCeil)
		local block_data = self._blocks[BLOCK_RES["block"..i]..j]
		print("new y", block_y, newPos.y - oldPos.y)
		-- 调整边界
		if block_y > self._blocks_map_size then
			block_y = self._blocks_map_size
		end

		local origin_y = block_data.y
		if block_y > origin_y or block_y < origin_y - len then
			if self._blocks_map[block_x + 1][block_y] == 1 then
				-- 调整边界，计算有误差
				if isCeil then
					block_y = block_y - 1
				else
					block_y = block_y + 1
				end
			end
		end

		-- 调整边界
		if block_y - len < 0 then
			block_y = 0 + len
		end

		if block_y > self._blocks_map_size - 1 then
			block_y = self._blocks_map_size
		end

		block_data.y = block_y
		block:setLocalZOrder(10 - block_y)--10 -v.y
		print(block_data.y, "move", "V")
		return oldPos.x, block_y* block_size - view_offset_y
	end
end

-- 清空移动前的block数据，然后再刷新
function GameView:resetBlockMap(dir, block_res_id, i, j, record)
	local v = self._blocks[BLOCK_RES["block"..i]..j]
	print("reset", table.tostring(v))
	local isRed = i == 1
	if dir == 0 then
		for idx = v.x , v.x + v.len - 1 do
			if idx+1 < self._blocks_map_size + 1 then
				self._blocks_map[idx + 1][v.y] = record
				-- print("reset",idx+1, v.y, record, "H")
			end
		end
	else
		for idx = v.y , v.y - v.len + 1, -1 do
			self._blocks_map[v.x + 1][idx] = record
			-- print("reset", v.x + 1, idx, record, "V")
		end
	end

end

function GameView:nextLevel()
	-- self:removeFromParent()
	local view = app:createView("TipsView", {func = function()
			print("start next level")
			local record_lv = gGame:getLevelRecord(gGame:getMode())
			if record_lv <50 then
				gGame:setLevelRecord(gGame:getMode(), record_lv + 1)
			end
			gGame:setLevel(gGame:getLevel() + 1)
			self:updateView()
			self._widgets.level_title:setString(string.format("%s-%d",gGame:getModeName(), gGame:getLevel() - (gGame:getMode()- 1)*50))
		end
	}
	)
	
	app:getScene():addChild(view)
end

function GameView:toHome()

end

function GameView:Integer(int)
	return math.floor(int)
end

return GameView
