open System
open System.IO

//
// Print Task Number
//
let TaskNumber (id: int, newLine: bool) = 
    if newLine = true then
        printfn ""
        printfn "%d. feladat:" id
    else
        printfn "%d. feladat:" id

//
// Solution of Task 1
//
let filePath = "ip.txt"
let data = File.ReadLines(filePath)

//
// Solution of Task 2
//
TaskNumber(2, false)
let count = data |> Seq.length
printfn "Az allomanyban %d darab adatsor van." count

//
// Solution of Task 3
//
TaskNumber(3, true)
let CharToInt (c: char) =
    if Char.IsDigit(c) = true
        then int c - int '0'
    else 10 + (int c - int 'a')

let IsColon (c: char) = 
    if c = ':' then true
    else false

let CreateValueFromIpAddress (str: string) =
    str |> Seq.map (fun x -> 
        if IsColon(x) = false then CharToInt(x)
        else 0
    ) |> Seq.sum

let ipAddressValues = data |> Seq.map CreateValueFromIpAddress |> Seq.toList
let smallestValue = ipAddressValues |> List.min
let indexOfSmallestValue = ipAddressValues |> List.findIndex (fun x -> x = smallestValue)
let smallestIpAddress = data |> Seq.toList |> List.item indexOfSmallestValue

printfn "A legalacsonyabb tarolt IP cim:"
printfn "%s" smallestIpAddress

//
// Solution of Task 4
//
TaskNumber(4, true)

let IpAddressType (str: string) =
    if str.[0..8] = "2001:0db8"
        then 0
    elif str.[0..6] = "2001:0e"
        then 1
    else 2

let addrTypes = data |> Seq.map IpAddressType

printfn "Dokumentacios cim: %d darab" (addrTypes |> Seq.where (fun x -> x = 0) |> Seq.length)
printfn "Globalis egyedi cim: %d darab" (addrTypes |> Seq.where (fun x -> x = 1) |> Seq.length)
printfn "Helyi egyedi cim: %d darab" (addrTypes |> Seq.where (fun x -> x = 2) |> Seq.length)

//
// Solution of Task 5
//
let EighteenZeroOrMore (str: string) =
    if (str |> Seq.map (fun x ->
        if x = '0' then 1
        else 0
    ) |> Seq.sum) >= 18
        then str
    else null

let zeroAddresses = data |> Seq.map EighteenZeroOrMore |> Seq.where (fun x -> not (x = null))
let zeroAddressesWithIndex = zeroAddresses |> Seq.map (fun x -> string ((data |> Seq.findIndex (fun y -> y = x)) + 1) + " " + x) 
File.WriteAllLines("sok.txt", zeroAddressesWithIndex)

//
// Solution of Task 6
//
TaskNumber(6, true)

let ReduceLeadingZeros (str: string) = 
    let splitted = str.Split ':'
    splitted |> Seq.map (fun x -> 
        if x = "0000"
            then "0"
        elif x.[0..2] = "000"
            then string x.[3]
        elif x.[0..1] = "00"
            then x.[2..3]
        elif x[0] = '0'
            then x.[1..3]
        else x
    ) |> String.concat ":"

printf "Kerek egy sorszamot: "
let chosenIndex = Console.ReadLine()
let chosen = data |> Seq.item (int chosenIndex - 1)

printfn "%s" chosen
printfn "%s" (ReduceLeadingZeros(chosen))

//
// Solution of Task 7
//
TaskNumber(7, true)

let ReduceZeroGroups (str: string) =
    if str = "0:0:0:0:0:0:0:0" then
        "::"
    elif str.Contains("0:0:0:0:0:0:0:") then
        str.Replace("0:0:0:0:0:0:0:", "::")
    elif str.Contains(":0:0:0:0:0:0:0") then
        str.Replace(":0:0:0:0:0:0:0", "::")
    elif str.Contains(":0:0:0:0:0:0:") then
        str.Replace(":0:0:0:0:0:0:", "::")
    elif str.Contains(":0:0:0:0:0:") then
        str.Replace(":0:0:0:0:0:", "::")
    elif str.Contains(":0:0:0:0:") then
        str.Replace(":0:0:0:0:", "::")
    elif str.Contains(":0:0:0:") then
        let location = str.IndexOf(":0:0:0:")
        str.Remove(location, 7).Insert(location, "::")
    elif str.Contains(":0:0:") then
        let location = str.IndexOf(":0:0:")
        str.Remove(location, 5).Insert(location, "::")
    else "Nem roviditheto tovabb."

printfn "%s" (ReduceZeroGroups(ReduceLeadingZeros(chosen)))