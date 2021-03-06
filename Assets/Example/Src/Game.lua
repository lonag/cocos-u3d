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
		tolua_error(tolua_S, "invalid 'cobj' in function 'lua_cocos2dx_Image_getColorAt'", nullptr);
		return 0;
	}
#endif

	argc = lua_gettop(tolua_S) - 1;
	if (argc == 3)
	{
#if COCOS2D_DEBUG >= 1
		int bitPerPixel = cobj->getBitPerPixel();
		if (bitPerPixel != 32) {
			tolua_error(tolua_S, "cc.Image:getColorAt ONLY work for Image 8888", nullptr);
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
	luaL_error(tolua_S, "%s has wrong number of arguments: %d, was expecting %d \n", "cc.Image:getColorAt", argc, 2);
	return 0;

#if COCOS2D_DEBUG >= 1
	tolua_lerror:
				tolua_error(tolua_S, "#ferror in function 'lua_cocos2dx_Image_getColorAt'.", &tolua_err);
#endif

				return 0;
}

local DIR = {
	VERTICAL = 1,
	HORIZON = 2,
	TOP_LEFT = 3,
	TOP_RIGHT = 4,
	BOTTOM_LEFT = 5,
	BOTTOM_RIGHT = 6
}

function Flow:ctor()
	self._blocks_map = {}
	self._record_blocks = {}
end

function Flow:upateCanvas()
	for i, k in pairs(self._record_blocks) do
		for j, v in pairs(k) do
			local x,y = v.x,v.y
			local image = ccui.ImageView:create()
			diplay.align(image, width/2, width/2)

			if v.dir == 2 then
				image:setScaleY(width)
			elseif v.dir == 1 then
				image:setScaleX(width)
			elseif v.dir == 3 then

			end

			image:setName("line"..x..y)
			self._unblock_bg:addChild(image, 10)
		end
	end
end

function Flow:upateView()
	self._flow = nil
	local i,j = 0,0
	local beganPos = nil
	local is_move = false
	self._block_circle = ccui.ImageView:create()

	self._flow:onTouch(function(event)
		if event.name == 'began' then
			beganPos = cc.p(event.target:getTouchBeganPos())
			local is_touch_block,block = self:getTouchBlock(beganPos)
			if is_touch_block then
				self._block_circle:setVisible(true)
				self._block_circle:setPosition(cc.p(block.x*width + offset_view_x, block.y*width + offset_view_y))
			end
			i = block.y - 1
			j = self._block_map_size + block.x - 1
			local is_touch_block = self:checkLinesOfBlockEnd(block)
			if not is_line_end_block then
				self:updateTouchBlock(block)
			else
				is_move = true
			end
			if not self._record_blocks["block"..i..j] then
				self._record_blocks["block"..i..j] = {}
			end
        elseif event.name == 'moved' then
        	if not is_move then
        		return
        	end
        	local movePos = cc.p(event.target:getTouchMovePos())
        	local is_record, x, y  = self:checkStartRecord(beganPos, width)
        	if not is_record then
        		return
        	end
        	self:updateRecordBlock(movePos,beganPos,width,i,j)

        	self._block_circle:setPosition(movePos)
		elseif event.name == 'cancelled' or event.name == 'ended' then
			self._block_circle:setVisible(false)
		end
	end)
end

function Flow:checkLinesOfBlockEnd(block)
	local line_id = block.line
	local x, y = block.x, block.y
	local records = self._record_blocks[line_id]
	local count = table.nums(records)
	if count == 0 then
		return false
	end
	local end_x, end_y = records[count].x, records[count].y
	if end_x == x and end_y == y then
		return true
	end

	return false
end

function Flow:checkStartRecord(beganPos, width)
	local x,y = self:getBlockIdex(beganPos, width)
	for i, v in pairs(self._blocks_map) do
		if v.x == x and y == v.y then
			return true,v
		end
	end
	return false
end

function Flow:updateRecordBlock(movePos,width,i,j)
	local dir = 0
	local x,y = self:getBlockIdex(movePos, width)
	local records = self._record_blocks["block"..i..j] 
	local have = false
	local pre_index = 1
	local new_record = {}
	for i, v in pairs(records) do
		table.insert(new_record, clone(v))
		if v.x == x and v.y == y then
			have = true
			pre_index = i -1
			break
		end
	end
	if not have then
		local pre_block = nil
		if pre_index > 0 then
			pre_block = records[pre_index]
		end
		if pre_block.x == x and pre_block.y ~= y then
			dir = DIR.VERTICAL
		elseif pre_block.y == y and pre_block.x ~= x then
			dir = DIR.VERTICAL
		end 
		-- calc lines to current block of point
		if pre_block.block_type >= 0 and v.block_type == 0 then
			self:updateRecordBlockByAstar(i, j)
		else
			table.insert(records, {block_type = 0, x = x, y = y, dir = dir, line = "block"..i..j})
			self:freshRecordBlock(i,j)
			-- new record
			self:updateCanvas()
		end
		
	else-- update record
		records = new_record
	end
end

function Flow:updateRecordBlockByAstar(i, j)

end

function Flow:freshRecordBlock(i,j)
	local block = self._blocks["block"..i..j]
	local block_x, block_y = block.x, block.y
	local end_x, end_y = 0,0
	local records = self._record_blocks["block"..i..j]
	local pre_block = nil
	local next_block = nil

	for i, v in pairs(records) do
		if i == 1 then
			pre_block = {x = block_x, y = block_y}
		else
			pre_block = records[i-1]
		end
		if i < table.nums(records) then
			next_block = records[i+1]
		else
			next_block = {x = end_x, y = end_y}
		end
		if v.x == pre_block.x and v.y == next_block.y then
			if pre_block.x < next_block.x and pre_block.y < next_block.y then
				v.dir = BOTTOM_RIGHT
			elseif pre_block.x < next_block.y and pre_block.y > next_block.y then
				v.dir = TOP_RIGHT
			elseif pre_block.x > next_block.x and pre_block.y < next_block.y then
				v.dir = BOTTOM_LEFT
			elseif pre_block.x > next_block.x and pre_block.y > next_block.y then
				v.dir = TOP_LEFT
			end
		elseif v.x == pre_block.x and v.y ~= pre_block.y then
			v.dir = VERTICAL
		elseif v.x ~= pre_block.x and v.y == pre_block.y then
			v.dir = HORIZON
		end
	end
end

function Flow:getLinesOfStartAndEndBlock()
	local lines = {}
	for i, v in pairs(self._blocks_map) do
		for j, v in pairs(v) do
			if not lines[v] then
				lines[v] = {}
			end
			table.insert(lines[v], {x= i-1, y = self._block_map_size+ i -1})
		end
	end
	return lines
end

function Flow:getTouchBlock(pos,width)
	local x, y = self:getBlockIdex(pos,width)
	for i, v in pairs(self._blocks_map) do
		if v.x == x and v.y == y then
			return true, x,y
		end
	end
	return false
end

function Flow:updateTouchBlock(block)
	local line_id = block.line
	local x = block.x
	local y = block.y
	local block_type = block.block_type
	local new_record = {}
	local records = self._record_blocks[line_id]
	for i,v in pairs(records) do
		table.insert(new_record, clone(v))
		if v.x == x and v.y == y then
			break
		end
	end
	records = new_record
end

function Flow:getBlockIdex(pos, width)
	local x = math.floor(pos.x/width) + 1
	local y = math.floor(pos.y/width)
end

function Flow:checkAllLine()
	local lines = self:getLinesOfStartAndEndBlock()
	for i, v in pairs(lines) do
		if not self:checkLineConnected(v[1],v[2]) then
			return false
		end
	end
	return true
end

function Flow:checkLineConnected(startPos, endPos)
	return false
end

--[[
        A*寻路算法，目前只适用于 01图, 0可通行， 1不可通行
--]]
 
-- 行走的4个方向
local four_dir = {
    {1, 0},
    {0, 1},
    {0, -1},
    {-1, 0},
}
 
-- 行走的8个方向
local eight_dir = {
    {1, 1},
    {1, 0},
    {1, -1},
    {0, 1},
    {0, -1},
    {-1, 1},
    {-1, 0},
    {-1, -1}
}
 
local AStar = {}
 
-- 地图、起始点、终点
AStar.init = function(self, map, startPoint, endPoint, four_dir)
    self.startPoint = startPoint
    self.endPoint   = endPoint
    self.map        = map
    self.cost       = 10 -- 单位花费
    self.diag       = 1.4 -- 对角线长， 根号2 一位小数
    self.open_list  = {}
    self.close_list = {}
    self.mapRows    = #map
    self.mapCols    = #map[1]
    self.four_dir   = four_dir -- 使用4方向的寻路还是八方向的寻路
end
 
-- 搜索路径 ,核心代码
AStar.searchPath = function(self)
    -- 验证终点是否为阻挡，如果为阻挡，则直接返回空路径
    if self.map[self.endPoint.row][self.endPoint.col] ~= 0 then
        Logger.info("(", self.endPoint.row, ",", self.endPoint.col, ") 是阻挡！！！无法寻路")
        return nil
    end

    -- 把第一节点加入 open_list中
    local startNode = {}  
    startNode.row = self.startPoint.row
    startNode.col = self.startPoint.col
    startNode.g = 0
    startNode.h = 0
    startNode.f = 0
    table.insert(self.open_list, startNode)
     
    -- 检查边界、障碍点 
    local check = function(row, col)
        if 1 <= row and row <= self.mapRows and 1 <= col and col <= self.mapCols then
                if self.map[row][col] == 0 or (row == self.endPoint.row and col == self.endPoint.col) then
                        return true
                end
        end

        return false
    end

    local dir = self.four_dir and four_dir or eight_dir
    while #self.open_list > 0 do
        local node = self:getMinNode()
        if node.row == self.endPoint.row and node.col == self.endPoint.col then
                -- 找到路径
                return self:buildPath(node)
        end

        for i = 1, #dir do
                local row = node.row + dir<i>[1]
                local col = node.col + dir<i>[2]
                if check(row, col) then
                        local curNode = self:getFGH(node, row, col, (row ~= node.row and col ~= node.col))
                        local openNode, openIndex = self:nodeInOpenList(row, col)
                        local closeNode, closeIndex = self:nodeInCloseList(row, col)

                        if not openNode and not closeNode then
                                -- 不在OPEN表和CLOSE表中
                                -- 添加特定节点到 open list
                                table.insert(self.open_list, curNode)
                        elseif openNode then
                                -- 在OPEN表
                                if openNode.f > curNode.f then
                                        -- 更新OPEN表中的估价值
                                        self.open_list[openIndex] = curNode
                                end
                        else
                                -- 在CLOSE表中
                                if closeNode.f > curNode.f then
                                        table.insert(self.open_list, curNode)
                                        table.remove(self.close_list, closeIndex)
                                end
                        end
                end
        end

        -- 节点放入到 close list 里面
        table.insert(self.close_list, node)
    end

    -- 不存在路径
    return nil
end
 
-- 获取 f ,g ,h, 最后参数是否对角线走
AStar.getFGH = function(self, father, row, col, isdiag)
    local node = {}
    local cost = self.cost
    if isdiag then
        cost = cost * self.diag
    end

    node.father = father
    node.row = row
    node.col = col
    node.g = father.g + cost
    -- 估计值h
    if self.four_dir then
        node.h = self:manhattan(row, col)
    else
        node.h = self:diagonal(row, col)
    end
    node.f = node.g + node.h  -- f = g + h 
    return node
end
 
-- 判断节点是否已经存在 open list 里面
AStar.nodeInOpenList = function(self, row, col)
    for i = 1, #self.open_list do
        local node = self.open_list<i>
        if node.row == row and node.col == col then
                return node, i   -- 返回节点和下标
        end
    end

    return nil
end
 
-- 判断节点是否已经存在 close list 里面
AStar.nodeInCloseList = function(self, row, col)
    for i = 1, #self.close_list do
        local node = self.close_list<i>
        if node.row == row and node.col == col then
                return node, i
        end
    end

    return nil
end
 
-- 在open_list中找到最佳点,并删除
AStar.getMinNode = function(self)
    if #self.open_list < 1 then
        return nil
    end

    local min_node = self.open_list[1]
    local min_i = 1
    for i,v in ipairs(self.open_list) do
        if min_node.f > v.f then
                min_node = v
                min_i = i
        end
    end

    table.remove(self.open_list, min_i)
    return min_node
end
 
-- 计算路径
AStar.buildPath = function(self, node)
	local path = {}
	local sumCost = node.f -- 路径的总花费
	while node do
        path[#path + 1] = {row = node.row, col = node.col}
        node = node.father
	end

	return path, sumCost
end
 
-- 估价h函数
-- 曼哈顿估价法（用于不能对角行走）
AStar.manhattan = function(self, row, col)  
    local h = math.abs(row - self.endPoint.row) + math.abs(col - self.endPoint.col)
    return h * self.cost
end
 
-- 对角线估价法,先按对角线走，一直走到与终点水平或垂直平行后，再笔直的走
AStar.diagonal = function(self, row, col)
	local dx = math.abs(row - self.endPoint.row)
	local dy = math.abs(col - self.endPoint.col)
	local minD = math.min(dx, dy)
	local h = minD * self.diag + dx + dy - 2 * minD
	return h * self.cost
end
 
return AStar