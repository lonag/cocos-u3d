
function Game:updateView()
	self._block_map= {
		{0,0,0,0,0,0},
		{0,0,0,0,0,0},
		{0,0,0,0,0,0},
		{0,0,0,0,0,0},
		{0,0,0,0,0,0},
		{0,0,0,0,0,0},
	}
	self.__record_block_orbit = {}
end

function Game:blockMoveEnded(beganPos, endPos, i, j)
	local pipe = self._blocks["pipe"..i..j]

	local is_right = endPos.x - beganPos.x > 0
	local is_top = endPos.y - beganPos.y > 0

	local old_x = pipe.x
	local old_y = pipe.y
	
	local new_x = 0
	local new_y = 0
	
	local vertical_offset = math.abs(endPos.y - beganPos.y)
	local horizon_offset = math.abs(endPos.x - beganPos.x)
	
	if vertical_offset > 10 then
		if is_top then
			if old_y + 1 > self._block_size then
				return false
			end
			if self._block_map[old_x][old_y + 1] == 0 then
				new_y = old_y + 1
			end
		else
			if old_y -1 == 0 then
				return false
			end
			if self._block_map[old_x][old_y - 1] == 0 then
				new_y = old_y - 1
			end
		else
	elseif horizon_offset > 10 then
		if is_right then
			if old_x + 1 > self._block_size then
				return false
			end
			if self._block_map[old_x + 1][old_y] == 0 then
				new_x = old_x + 1
			end
		else
			if old_x - 1 < 0 then
				return false
			end
			if self._block_map[old_x - 1][old_y] == 0 then
				new_x = old_x - 1
			end
		end
	end
	return true, new_x, new_y
end

local PIPE_TYPE = {
	TOP = 1,
	BOTTOM = 2,
	LEFT = 3,
	RIGHT = 4,
	TOP_LEFT = 5,
	TOP_RIGHT = 6,
	BOTTOM_LEFT = 7,
	BOTTOM_RIGHT = 8,
	LEFT_RIGHT = 9,
	TOP_BOTTOM = 10,
}

function Game:getStartBlock()
	return x, y, pipe_type
end

function Game:getEndBlock()
	return x, y, pipe_type
end

function Game:getBlock(i,j)
	return x, y, pipe_type
end

function Game:getBallOrbitActions()
	local actions = {}
	local count = table.nums(self._record_block_orbit)
	for i=1, count do
		local next_block = {}
		if i == count then
			next_block.x = 0
			next_block.y = 0
		else
			next_block = self._record_block_orbit[i+1]
		end

		local current_block = self._record_block_orbit[i]
		local action = nil
		if current_block.pipe_type == TOP_LEFT or current_block.pipe_type == TOP_RIGHT or current_block.pipe_type = BOTTOM_LEFT or current_block.pipe_type == BOTTOM_RIGHT then
			action = cc.BezierTo(1, {
				-- endPosition
				cc.p(next_block.x, next_block.y), 
				-- controlPoint_1
				cc.p(), 
				-- controlPoint_2 
				cc.p()})
		else
			action = cc.MoveTo:create(1, cc.p(next_block.x, next_block.y))
		end
		table.insert(actions, action)
		
	end

	local action = cc.CallFunc:create(function()

		end)
	table.insert(actions, action)
end

function Game:checkBlockConnected(start_x, start_y, start_pipe_type)
	local end_x, end_y, end_pipe_type = self:getEndBlock()

	if start_pipe_type == PIPE_TYPE.TOP then
		if end_x ~= start_x or end_y ~= start_y + 1 then
			return false
		end
		if block_type == PIPE_TYPE.TOP_BOTTOM or block_type == PIPE_TYPE.BOTTOM_LEFT or block_type == PIPE_TYPE.BOTTOM_RIGHT then
			true
		end
	elseif start_pipe_type == PIPE_TYPE.BOTTOM then
		if end_x ~= start_x or end_y ~= start_y - 1 then
			return false
		end
		if block_type == PIPE_TYPE.TOP_BOTTOM or block_type == PIPE_TYPE.TOP_LEFT or block_type == PIPE_TYPE.TOP_RIGHT then
			return true
		end
	elseif start_pipe_type == PIPE_TYPE.LEFT or start_pipe_type == PIPE_TYPE.TOP_LEFT or start_pipe_type == PIPE_TYPE.BOTTOM_LEFT or start_pipe_type == PIPE_TYPE.LEFT_RIGHT then
		if end_x ~= start_x - 1 or end_y ~= start_y then
			return false
		end
		if block_type == PIPE_TYPE.LEFT_RIGHT or block_type == PIPE_TYPE.BOTTOM_RIGHT or block_type == PIPE_TYPE.TOP_RIGHT then
			return true
		end
	elseif start_pipe_type == PIPE_TYPE.RIGHT or start_pipe_type == PIPE_TYPE.TOP_RIGHT or start_pipe_type == PIPE_TYPE.BOTTOM_RIGHT or start_pipe_type == PIPE_TYPE.LEFT_RIGHT then
		if end_x ~= start_x + 1 or end_y ~= start_y then
			return false
		end
		if block_type == PIPE_TYPE.LEFT_RIGHT or block_type == PIPE_TYPE.BOTTOM_LEFT or block_type == PIPE_TYPE.TOP_LEFT then
			return true
		end
	end 
	return false
end

function Game:checkBlockConnect(start_x, start_y, start_pipe_type)
	table.insert(self._record_block_orbit, {x = start_x, y = start_y, pipe_type = start_pipe_type})
	if self:checkBlockConnected(start_x, start_y, start_pipe_type) then
		return true
	end
	if start_pipe_type == PIPE_TYPE.TOP then
		if start_y == self._block_size then
			return false
		end
		local block_x, block_y, block_type = self:getBlock(start_x,start_y + 1)
		if block_type == PIPE_TYPE.TOP_BOTTOM or block_type == PIPE_TYPE.BOTTOM_LEFT or block_type == PIPE_TYPE.BOTTOM_RIGHT then
			return self:checkBlockConnect(block_x , block_y, block_type)
		end
	elseif start_pipe_type == PIPE_TYPE.BOTTOM then
		if start_y - 1 == 0 then
			return false
		end
		local block_x, block_y, block_type = self:getBlock(start_x,start_y - 1)
		if block_type == PIPE_TYPE.TOP_BOTTOM or block_type == PIPE_TYPE.TOP_LEFT or block_type == PIPE_TYPE.TOP_RIGHT then
			return self:checkBlockConnect(block_x , block_y, block_type)
		end
	elseif start_pipe_type == PIPE_TYPE.LEFT or start_pipe_type == PIPE_TYPE.TOP_LEFT or start_pipe_type == PIPE_TYPE.BOTTOM_LEFT or start_pipe_type == PIPE_TYPE.LEFT_RIGHT then
		if start_x - 1 == 0 then
			return false
		end
		local block_x, block_y, block_type = self:getBlock(start_x - 1,start_y)
		if block_type == PIPE_TYPE.LEFT_RIGHT or block_type == PIPE_TYPE.BOTTOM_RIGHT or block_type == PIPE_TYPE.TOP_RIGHT then
			return self:checkBlockConnect(block_x , block_y, block_type)
		end
	elseif start_pipe_type == PIPE_TYPE.RIGHT or start_pipe_type == PIPE_TYPE.TOP_RIGHT or start_pipe_type == PIPE_TYPE.BOTTOM_RIGHT or start_pipe_type == PIPE_TYPE.LEFT_RIGHT then
		if start_x == self._block_size then
			return false
		end
		local block_x, block_y, block_type = self:getBlock(start_x + 1,start_y)
		if block_type == PIPE_TYPE.LEFT_RIGHT or block_type == PIPE_TYPE.BOTTOM_LEFT or block_type == PIPE_TYPE.TOP_LEFT then
			return self:checkBlockConnect(block_x , block_y, block_type)
		end
	end 
	return false
end