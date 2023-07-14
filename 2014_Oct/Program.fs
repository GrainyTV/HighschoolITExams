open System
open System.IO
open System.Text.RegularExpressions

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
let reservation = File.ReadLines("foglaltsag.txt")
let category = File.ReadLines("kategoria.txt")

//
// Solution of Task 2
//
TaskNumber(2, false)

let AvailableChair (chair: char) = if chair = 'o' then true else false

printf "Adja meg egy sor szamat: "
let providedRowNumber = Console.ReadLine()
printf "Adja meg a sor egy szekenek szamat: "
let providedChairNumber = Console.ReadLine()

printfn "A %s. sor %s. szeke %s." 
    providedRowNumber
    providedChairNumber
    (if AvailableChair((reservation |> Seq.item (int providedRowNumber - 1))[int providedChairNumber - 1]) = true then "szabad" else "foglalt")

//
// Solution of Task 3
//
TaskNumber(3, true)

let soldTicketAmount = reservation |> Seq.map (fun row -> (row |> Seq.map (fun chair -> if chair = 'x' then 1 else 0)) |> Seq.sum) |> Seq.sum
let theatreCapacity = 15.0 * 20.0

printfn "Az eloadasra eddig %d jegyet adtak el, ez a nezoter %.0f%%-a." soldTicketAmount (float soldTicketAmount / theatreCapacity * 100.0)

//
// Solution of Task 4
//
TaskNumber(4, true)

let reservationList = reservation |> Seq.map (fun row -> row |> Seq.map (fun chair -> chair) |> Seq.toList) |> List.concat
let categoryList = category |> Seq.map (fun row -> row |> Seq.map (fun pricerange -> pricerange) |> Seq.toList) |> List.concat
let chairWithPriceRange = (reservationList, categoryList) ||> List.map2 (fun chair pricerange -> (chair, pricerange))

let firstClass = chairWithPriceRange |> List.where (fun value -> fst(value) = 'x' && snd(value) = '1') |> List.length
let secondClass = chairWithPriceRange |> List.where (fun value -> fst(value) = 'x' && snd(value) = '2') |> List.length
let thirdClass = chairWithPriceRange |> List.where (fun value -> fst(value) = 'x' && snd(value) = '3') |> List.length
let forthClass = chairWithPriceRange |> List.where (fun value -> fst(value) = 'x' && snd(value) = '4') |> List.length
let fifthClass = chairWithPriceRange |> List.where (fun value -> fst(value) = 'x' && snd(value) = '5') |> List.length

let classList = [firstClass; secondClass; thirdClass; forthClass; fifthClass]
printfn "A legtobb jegyet a(z) %d. arkategoriaban ertekesitettek." ((classList |> List.findIndex (fun value -> value = (classList |> List.max))) + 1)

//
// Solution of Task 5
//
TaskNumber(5, true)

let prices = [5000; 4000; 3000; 2000; 1500]
let income = 
    firstClass * prices[0]
    +
    secondClass * prices[1]
    +
    thirdClass * prices[2]
    +
    forthClass * prices[3]
    +
    fifthClass * prices[4]

printfn "A szinhaz jelenlegi bevetele: %d Ft." income

//
// Solution of Task 6
//
TaskNumber(6, true)

let EmptySoloChairs (row: string) = 
    let leftSide = 
        if row[0] = 'o' && row[1] = 'x' then 1
        else 0
    
    let rightSide =
        if row[row.Length - 1] = 'o' && row[row.Length - 2] = 'x' then 1
        else 0

    let middle = Regex.Matches(row, "(?<=(xox))").Count

    leftSide + middle + rightSide

printfn "%d db egyedulallo ures hely maradt." (reservation |> Seq.map EmptySoloChairs |> Seq.sum)

//
// Solution of Task 7
//
let outputText = chairWithPriceRange |> Seq.map (fun value -> if fst(value) = 'x' then 'x' else snd(value)) |> Seq.splitInto 15
let concatenatedOutputText = outputText |> Seq.map (fun value -> String.Concat(value))

File.WriteAllLines("szabad.txt", concatenatedOutputText)