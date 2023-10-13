const std = @import("std");
const print = std.debug.print;
const workingDir = std.fs.cwd();

pub fn dynamicSearch(elements: *[][]const u8, index: usize, direction: u8) u8 {
    if (index < 0 and index > 7) {
        return 0;
    } else if (std.mem.eql(u8, elements.*[index], "0") and direction == '0') {
        return 1 + dynamicSearch(elements, index - 1, '-') + dynamicSearch(elements, index + 1, '+');
    } else if (std.mem.eql(u8, elements.*[index], "0") and direction == '-') {
        return 1 + dynamicSearch(elements, index - 1, '-');
    } else if (std.mem.eql(u8, elements.*[index], "0") and direction == '+') {
        return 1 + dynamicSearch(elements, index + 1, '+');
    } else {
        return 0;
    }
}

pub fn forceAbbreviate(allocator: *const std.mem.Allocator, ip: *const []const u8) ![]const u8 {
    var splittedAddress = std.mem.splitSequence(u8, ip.*, ":");
    var buffer = std.ArrayList([]const u8).init(allocator.*);

    while (splittedAddress.next()) |addressPart| {
        try buffer.append(addressPart);
    }

    var i: usize = 0;
    var longest: u8 = 0;

    for (buffer.items, 0..) |_, index| {
        const amount = dynamicSearch(&buffer.items, index, '0');

        if (amount > longest and amount > 1) {
            longest = amount;
            i = index;
        }
    }

    for (0..longest) |index| {
        if (index == longest - 1) {
            buffer.items[i] = "";
        } else {
            _ = buffer.orderedRemove(i);
        }
    }

    return try std.mem.join(allocator.*, ":", try buffer.toOwnedSlice());
}

pub fn removeInitialZeros(allocator: *const std.mem.Allocator, ip: *const []const u8) ![]const u8 {
    var requiredParts = std.ArrayList([]const u8).init(allocator.*);
    var splittedAddress = std.mem.splitSequence(u8, ip.*, ":");

    while (splittedAddress.next()) |addressPart| {
        var terminateCondition = false;

        for (addressPart, 1..) |alphanumeric, count| {
            if (terminateCondition == false and alphanumeric == '0' and count < 4) {
                continue;
            } else {
                terminateCondition = true;
                var currentValue = try allocator.dupe(u8, &[_]u8{ alphanumeric });
                try requiredParts.append(currentValue);
            }
        }

        var delimiter = try allocator.dupe(u8, &[_]u8{ ':' });
        try requiredParts.append(delimiter);
    }

    requiredParts.shrinkAndFree(requiredParts.items.len - 1);
    return try std.mem.concat(allocator.*, u8, try requiredParts.toOwnedSlice());
}

pub fn moreThanSeventeenZeros(ip: *const []const u8) bool {
    var counter: u32 = 0;

    for (ip.*) |alphanumeric| {
        if (alphanumeric == '0') {
            counter += 1;
        }
    }

    return if (counter >= 18) true else false;
}

pub fn main() !void {
//
// Task : 1
//
    const inputFile = try workingDir.openFile("ip.txt", .{});
    defer inputFile.close();
    
    var arena = std.heap.ArenaAllocator.init(std.heap.page_allocator);
    defer arena.deinit();
    const allocator = arena.allocator(); 
    
    var ipAddresses = std.ArrayList([]const u8).init(allocator); 
    var bufferedReader = std.io.bufferedReaderSize(41, inputFile.reader());
    var reader = bufferedReader.reader();

    while (try reader.readUntilDelimiterOrEof(&bufferedReader.buf, '\n')) |line| {
        const trimmedLine = std.mem.trimRight(u8, line, "\r");
        var heapBuffer = try allocator.dupe(u8, trimmedLine);
        try ipAddresses.append(heapBuffer);
    }
//
// Task : 2
//
    print("2. feladat:\n", .{});
    print("Az allomanyban {d} darab adatsor van.\n\n", .{ipAddresses.items.len});
//
// Task : 3
//
    var lowestAddress: []const u8 = undefined;
    var best: u32 = std.math.maxInt(u32);

    for (ipAddresses.items) |item| {
        var counter: u32 = 0;

        for (item) |alphanumeric| {
            counter += alphanumeric;
        }

        if (counter < best) {
            best = counter;
            lowestAddress = item;
        }
    }

    print("3. feladat:\n", .{});
    print("A legalacsonyabb tarolt IP-cim:\n", .{});
    print("{s}\n\n", .{lowestAddress});
//
// Task : 4
//
    var ipTypesAmount = [_]u32{ 0, 0, 0 };

    for (ipAddresses.items) |item| {
        if (std.mem.eql(u8, item[0..9], "2001:0db8")) {
            ipTypesAmount[0] += 1;
        } else if (std.mem.eql(u8, item[0..7], "2001:0e")) {
            ipTypesAmount[1] += 1;
        } else {
            ipTypesAmount[2] += 1;
        }
    }    

    print("4. feladat:\n", .{});
    print("Dokumentacios cim: {d} darab\n", .{ipTypesAmount[0]});
    print("Globalis egyedi cim: {d} darab\n", .{ipTypesAmount[1]});
    print("Helyi egyedi cim: {d} darab\n\n", .{ipTypesAmount[2]});
//
// Task : 5
//
    const outputFile = try workingDir.createFile("sok.txt", .{});
    defer outputFile.close();

    var writer = outputFile.writer();

    for (ipAddresses.items, 1..) |item, index| {
        if (moreThanSeventeenZeros(&item) == true) {
            try std.fmt.format(writer, "{d} {s}\n", .{index, item});
        }
    }
//
// Task : 6
//
    print("6. feladat:\n", .{});
    print("Kerek egy sorszamot: ", .{});
    
    const stdin = std.io.getStdIn().reader();
    const userInput = try stdin.readUntilDelimiterOrEof(&bufferedReader.buf, '\n');
    var chosenAddress: []const u8 = undefined;

    if (userInput) |index| {
        chosenAddress = ipAddresses.items[(std.fmt.parseUnsigned(u32, index, 10) catch 1) - 1];
    } else {
        chosenAddress = ipAddresses.items[0];
    }

    const abbreviatedAddress = try removeInitialZeros(&allocator, &chosenAddress);

    print("{s}\n", .{chosenAddress});
    print("{s}\n\n", .{abbreviatedAddress});
//
// Task : 7
//
    const evenMoreAbbreviatedAddress = try forceAbbreviate(&allocator, &abbreviatedAddress);
    print("7. feladat:\n", .{});

    if(std.mem.eql(u8, abbreviatedAddress, evenMoreAbbreviatedAddress)) {
        print("Nem roviditheto tovabb.\n", .{});
    } else {
        print("{s}\n", .{evenMoreAbbreviatedAddress});
    }  
}
